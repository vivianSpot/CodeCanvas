using CodeCanvas.Entities;
using System.Threading.Tasks;

namespace CodeCanvas.Services
{
	public interface IWalletAdjustmentService
	{
		Task<decimal> AdjustBalance(string exchangeRateStrategy, int walletId, string currencyCode, decimal amount);
		Task CreateWallet(WalletEntity walletEntity);
		Task<WalletEntity> GetWallet(int id);
	}
}
