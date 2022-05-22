using CodeCanvas.Repositories;
using System;
using System.Threading.Tasks;

namespace CodeCanvas.ExchangeRateStrategies
{
    // 1) SpecificDateExchangeRateStrategy: should find the rate for the specified date,
    //		or an exception must be thrown in case date is missing 
    public class SpecificDateExchangeRateStrategy : ExchangeRateStrategyBase
    {
        public SpecificDateExchangeRateStrategy(ICurrencyRateRepository currencyRateRepository) : base(currencyRateRepository)
        {
        }

        protected override async Task<decimal> GetRate(string currencyCodeFrom, string currencyCodeTo, DateTime date)
        {
            var rateFrom = await GetCurrencyRateEntry(date, currencyCodeFrom);
            if (rateFrom == null)
                throw new Exception($"Rate missing for date '{date}' and currency code '{currencyCodeFrom}'");

            var rateTo = await GetCurrencyRateEntry(date, currencyCodeTo);
            if (rateTo == null)
                throw new Exception($"Rate missing for date '{date}' and currency code '{currencyCodeTo}'");

            return rateTo.Rate / rateFrom.Rate;

        }
    }
}
