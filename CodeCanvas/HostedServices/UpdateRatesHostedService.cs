using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeCanvas.HostedServices
{
	public class UpdateRatesHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<UpdateRatesHostedService> _logger;
		private Timer? _timer;

		public UpdateRatesHostedService(ILogger<UpdateRatesHostedService> logger)
		{
			_logger = logger;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("UpdateRatesHostedService running.");

			_timer = new Timer(UpdateRates, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		private void UpdateRates(object? state)
		{
			// todo: implement UpdateRatesHostedService.UpdateRates()

			// 1) use IEuropeanCentralBankClient.GetRates() to fetch the latest rates

			// 2) we should keep only one rate record for each date (day)...
			//		so we update entries in database for the same date (day),
			//		but we add a new record in the database when date (day) changes

			_logger.LogInformation("UpdateRatesHostedService rates updated.");

			throw new NotImplementedException();
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
