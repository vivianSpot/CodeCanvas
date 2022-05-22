using CodeCanvas.Database;
using CodeCanvas.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCanvas.Repositories
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CurrencyRateRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<CurrencyRateEntity> GetCurrencyRateByDateAndCurrencyCodeAsync(DateTime date, string currencyCode)
        {
            return await _applicationDbContext.CurrencyRates.SingleOrDefaultAsync(x => x.CreatedAt.Date == date.Date && x.CurrencyCode == currencyCode);
        }

        public IEnumerable<CurrencyRateEntity> GetAllByDate(DateTime date)
        {
            return _applicationDbContext.CurrencyRates.Where(x => x.CreatedAt.Date == date.Date);
        }

        public void InsertCurrencyRate(CurrencyRateEntity currencyRate)
        {
            _applicationDbContext.CurrencyRates.Add(currencyRate);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public void Update(CurrencyRateEntity currencyRate)
        {
            _applicationDbContext.Entry(currencyRate).State = EntityState.Modified;
        }
    }
}
