using CodeCanvas.Repositories;
using System;
using System.Threading.Tasks;

namespace CodeCanvas.ExchangeRateStrategies
{
    // 2) SpecificDateOrNextAvailableRateStrategy: should find the rate for the specified date,
    //		or the rate of the next available date after the specified date,
    //		again an exception must be thrown in case no available date found
    public class SpecificDateOrNextAvailableRateStrategy : ExchangeRateStrategyBase
    {
        public SpecificDateOrNextAvailableRateStrategy(ICurrencyRateRepository currencyRateRepository) : base(currencyRateRepository)
        {
        }

        protected override async Task<decimal> GetRate(string currencyCodeFrom, string currencyCodeTo, DateTime date)
        {
            var rateFrom = await GetCurrencyRateEntry(date, currencyCodeFrom) ?? await GetCurrencyRateEntry(date.AddDays(1), currencyCodeFrom);
            if (rateFrom == null)
                throw new Exception($"Rate missing for date '{date}' and currency code '{currencyCodeFrom}'");

            var rateTo = await GetCurrencyRateEntry(date, currencyCodeTo) ?? await GetCurrencyRateEntry(date.AddDays(1), currencyCodeTo);
            if (rateTo == null)
                throw new Exception($"Rate missing for date '{date}' and currency code '{currencyCodeTo}'");

            return rateTo.Rate / rateFrom.Rate;
        }
    }
}
