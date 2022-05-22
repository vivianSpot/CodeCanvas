using CodeCanvas.Entities;
using CodeCanvas.Repositories;
using System;
using System.Threading.Tasks;

namespace CodeCanvas.ExchangeRateStrategies
{
    public abstract class ExchangeRateStrategyBase : IExchangeRateStrategy
	{
		private readonly ICurrencyRateRepository _currencyRateRepository;

		public ExchangeRateStrategyBase(ICurrencyRateRepository currencyRateRepository)
		{ 
			_currencyRateRepository = currencyRateRepository;
		}

		public async Task<decimal> Convert(decimal amount, string amountCurrencyCode, string currencyCodeToConvert, DateTime date)
		{
			var rate = await GetRate(amountCurrencyCode, currencyCodeToConvert, date);
			return amount * rate;
		}

		protected async Task<CurrencyRateEntity> GetCurrencyRateEntry(DateTime date, string currencyCode)
		{
			return await _currencyRateRepository.GetCurrencyRateByDateAndCurrencyCodeAsync(date, currencyCode);
		}

		// created two derived classes from ExchangeRateStrategyBase and override GetRate()
		protected abstract Task<decimal> GetRate(string currencyCodeFrom, string currencyCodeTo, DateTime date);
	}
}
