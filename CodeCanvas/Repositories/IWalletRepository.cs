using CodeCanvas.Entities;
using System.Threading.Tasks;

namespace CodeCanvas.Repositories
{
    public interface IWalletRepository
    {
        void Update(WalletEntity wallet);
        Task<bool> SaveAllAsync();
        Task<WalletEntity> GetWalletByIdAsync(int id);
        void InsertWallet(WalletEntity wallet);
    }
}
