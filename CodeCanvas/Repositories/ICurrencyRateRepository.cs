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
        IEnumerable<CurrencyRateEntity> GetCurrencyRatesByDateAsync(DateTime date);
    }
}
