using CodeCanvas.Database;
using CodeCanvas.Entities;
using CodeCanvas.Repositories;
using EuropeanCentralBank;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeCanvas.HostedServices
{
    public class UpdateRatesHostedService : IHostedService, IDisposable
	{
		private readonly IEuropeanCentralBankClient _europeanCentralBankClient;
		private readonly ICurrencyRateRepository _currencyRateRepository;
		//private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<UpdateRatesHostedService> _logger;
		private Timer? _timer;

		public UpdateRatesHostedService(IEuropeanCentralBankClient europeanCentralBankClient, ICurrencyRateRepository currencyRateRepository, ILogger<UpdateRatesHostedService> logger)
		{
			_europeanCentralBankClient = europeanCentralBankClient;
			_currencyRateRepository = currencyRateRepository;
			//_scopeFactory = scopeFactory;
			_logger = logger;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("UpdateRatesHostedService running.");

			_timer = new Timer(UpdateRates, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		private async void UpdateRates(object? state)
		{
			// 1) use IEuropeanCentralBankClient.GetRates() to fetch the latest rates
			var latestRates = await _europeanCentralBankClient.GetRates();

			if (latestRates?.Rates == null)
				throw new Exception("No rates found.");

			// 2) we should keep only one rate record for each date (day)...
			//		so we update entries in database for the same date (day),
			//		but we add a new record in the database when date (day) changes

			var ratesDictionary = latestRates.Rates.ToDictionary(x => x.CurrencyCode, x => x.Rate);

			//using (var scope = _scopeFactory.CreateScope())
			//{
				//var _applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				//we only update rates with latestRates's day and if the Rate has changed for a CurrencyCode
				var ratesToUpdate = _applicationDbContext.CurrencyRates.ToList().Where(x => x.CreatedAt.Date == latestRates.Date)
					.Where(x => latestRates.Rates.Any(r => r.CurrencyCode == x.CurrencyCode && r.Rate != x.Rate));

				foreach (var rate in ratesToUpdate)
				{
					rate.Update(ratesDictionary[rate.CurrencyCode]);
					_applicationDbContext.CurrencyRates.Update(rate);
				}
				
				//insert Rates if no row with CurrencyCode exists for latestRates's day
				var ratesToInsert = latestRates.Rates.Where(x => !_applicationDbContext.CurrencyRates.Any(r => r.CurrencyCode == x.CurrencyCode && latestRates.Date == r.CreatedAt.Date));
				foreach (var rate in ratesToInsert)
				{
					await _applicationDbContext.CurrencyRates.AddAsync(new CurrencyRateEntity(rate.CurrencyCode, rate.Rate, latestRates.Date));
				}
			//}

			_logger.LogInformation("UpdateRatesHostedService rates updated.");
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("UpdateRatesHostedService is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}

