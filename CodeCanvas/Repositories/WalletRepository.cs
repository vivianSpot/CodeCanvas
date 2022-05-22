using CodeCanvas.Database;
using CodeCanvas.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CodeCanvas.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public WalletRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<WalletEntity> GetWalletByIdAsync(int id)
        {
            return await _applicationDbContext.FindAsync<WalletEntity>(id);
        }

        public void InsertWallet(WalletEntity wallet)
        {
            _applicationDbContext.Wallets.Add(wallet);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public void Update(WalletEntity wallet)
        {
            _applicationDbContext.Entry(wallet).State = EntityState.Modified;
        }
    }
}
