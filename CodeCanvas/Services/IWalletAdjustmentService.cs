using CodeCanvas.Entities;
using System.Threading.Tasks;

namespace CodeCanvas.Services
{
	public interface IWalletAdjustmentService
	{
		Task<decimal> AdjustBalance(string exchangeRateStrategy, int walletId, string currencyCode, decimal amount);
		Task CreateWalletAsync(WalletEntity walletEntity);
		Task<WalletEntity> GetWalletAsync(int id);
	}
}
