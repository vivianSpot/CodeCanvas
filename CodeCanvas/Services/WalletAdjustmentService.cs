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
			IExchangeRateStrategy _strategy;
			// choose the corresponding IExchangeRateStrategy based on exchangeRateStrategy
			if (exchangeRateStrategy == "SpecificDateExchangeRateStrategy")
			{
				_strategy = new SpecificDateExchangeRateStrategy(_currencyRateRepository);
			}
			else
			{
				_strategy = new SpecificDateOrNextAvailableRateStrategy(_currencyRateRepository);
			}

			//find wallet by id
			var wallet = await _walletRepository.GetWalletByIdAsync(walletId);

			if (wallet == null)
				throw new Exception($"No wallet found for id {walletId}");

			// use IExchangeRateStrategy.Convert() to convert the amount into the currency of the wallet
			var convertedAmount = await _strategy.Convert(amount, currencyCode, wallet.CurrencyCode, DateTime.UtcNow);

			// bring WalletEntity and call Adjust() to adjust the balance
			wallet.Adjust(convertedAmount);
			
			// persist the changes
			await _walletRepository.SaveAllAsync();

			// retun new balance
			return wallet.Balance;
		}

		public async Task CreateWalletAsync(WalletEntity walletEntity)
		{
			_walletRepository.InsertWallet(walletEntity);
			await _walletRepository.SaveAllAsync();
		}

		public async Task<WalletEntity> GetWalletAsync(int id)
		{
			return await _walletRepository.GetWalletByIdAsync(id);
		}
	}
}
