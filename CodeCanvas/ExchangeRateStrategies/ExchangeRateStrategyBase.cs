using System;
using System.Threading.Tasks;

namespace CodeCanvas.ExchangeRateStrategies
{
	abstract class ExchangeRateStrategyBase : IExchangeRateStrategy
	{
		public async Task<decimal> Convert(decimal amount, string amountCurrencyCode, string currencyCodeToConvert, DateTime date)
		{
			var rate = await GetRate(amountCurrencyCode, currencyCodeToConvert, date);
			return amount * rate;
		}

		protected abstract Task<decimal> GetRate(string currencyCodeFrom, string currencyCodeTo, DateTime date);

		// todo: create two derived classes from ExchangeRateStrategyBase and override GetRate()

		// 1) SpecificDateExchangeRateStrategy: should find the rate for the specified date,
		//		or an exception must be thrown in case date is missing 

		// 2) SpecificDateOrNextAvailableRateStrategy: should find the rate for the specified date,
		//		or the rate of the next available date after the specified date,
		//		again an exception must be thrown in case no available date found
	}
}
