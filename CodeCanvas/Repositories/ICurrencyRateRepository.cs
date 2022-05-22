using CodeCanvas.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCanvas.Repositories
{
    public interface ICurrencyRateRepository
    {
        void Update(CurrencyRateEntity currencyRate);
        Task<bool> SaveAllAsync();
        IEnumerable<CurrencyRateEntity> GetAllByDate(DateTime date);
        Task<CurrencyRateEntity> GetCurrencyRateByDateAndCurrencyCodeAsync(DateTime date, string currencyCode);
        void InsertCurrencyRate(CurrencyRateEntity currencyRate);
    }
}
