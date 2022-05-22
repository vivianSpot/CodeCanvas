using CodeCanvas.Entities;
using CodeCanvas.ExchangeRateStrategies;
using CodeCanvas.Repositories;
using System;
using System.Threading.Tasks;

namespace CodeCanvas.Services
{
	public class WalletAdjustmentService : IWalletAdjustmentService
	{
		private readonly ICurrencyRateRepository _currencyRateRepository;
		private readonly IWalletRepository _walletRepository;

		public WalletAdjustmentService(ICurrencyRateRepository currencyRateRepository, IWalletRepository walletRepository)
		{
			_currencyRateRepository = currencyRateRepository;
			_walletRepository = walletRepository;
		}

		public async Task<decimal> AdjustBalance(string exchangeRateStrategy, int walletId, string currencyCode, decimal amount)
		{
			// choose the corresponding IExchangeRateStrategy based on exchangeRateStrategy
			IExchangeRateStrategy strategy;

			if (exchangeRateStrategy == "SpecificDateExchangeRateStrategy")
			{
				strategy = new SpecificDateExchangeRateStrategy(_currencyRateRepository);
			}
			else
			{
				strategy = new SpecificDateOrNextAvailableRateStrategy(_currencyRateRepository);
			}

			//find wallet by id
			var wallet = await _walletRepository.GetWalletByIdAsync(walletId);

			if (wallet == null)
				throw new Exception($"No wallet found for id {walletId}");

			// use IExchangeRateStrategy.Convert() to convert the amount into the currency of the wallet
			var convertedAmount = await strategy.Convert(amount, currencyCode, wallet.CurrencyCode, DateTime.UtcNow.AddDays(-2));

			// bring WalletEntity and call Adjust() to adjust the balance
			wallet.Adjust(convertedAmount);
			
			// persist the changes
			await _walletRepository.SaveAllAsync();

			// retun new balance
			return convertedAmount;
		}

		public async Task CreateWallet(WalletEntity walletEntity)
		{
			_walletRepository.InsertWallet(walletEntity);
			await _walletRepository.SaveAllAsync();
		}

		public async Task<WalletEntity> GetWallet(int id)
		{
			return await _walletRepository.GetWalletByIdAsync(id);
		}
	}
}
