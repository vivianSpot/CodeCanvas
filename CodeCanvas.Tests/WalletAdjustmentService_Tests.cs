using CodeCanvas.Entities;
using CodeCanvas.ExchangeRateStrategies;
using CodeCanvas.Repositories;
using CodeCanvas.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodeCanvas.Tests
{
    public class WalletAdjustmentService_Tests
    {
        private readonly Mock<ICurrencyRateRepository> _currencyRateRepository;
        private readonly Mock<IWalletRepository> _walletRepository;
        private readonly WalletAdjustmentService walletAdjustmentService;
        public WalletAdjustmentService_Tests()
        {
            _currencyRateRepository = new Mock<ICurrencyRateRepository>();
            _walletRepository = new Mock<IWalletRepository>();

            walletAdjustmentService = new WalletAdjustmentService(_currencyRateRepository.Object, _walletRepository.Object);
        }

        [Fact]
        public void AdjustBalance_NoWallet_Throws()
        {
            var ex = Assert.ThrowsAsync<Exception>(() => walletAdjustmentService.AdjustBalance("SpecificDateExchangeRateStrategy", 0, "USD", 5));

            Assert.Equal("No wallet found for id 0", ex.Result.Message);
        }

        //[Theory]
        //[InlineData("SpecificDateExchangeRateStrategy")]
        //[InlineData("SpecificDateOrNextAvailableRateStrategy")]
        //public async void AdjustBalance_Convert_Throws(string strategy)
        //{
        //    _walletRepository.Setup(x => x.GetWalletByIdAsync(1))
        //        .ReturnsAsync(new WalletEntity(1, "GBP", 2, DateTime.UtcNow, DateTime.UtcNow));

        //    var mock = new Mock<ExchangeRateStrategyBaseMock>();

        //    var result = await walletAdjustmentService.AdjustBalance(strategy, 1, "USD", 5);

        //    Assert.Equal()
        //}

        [Theory]
        [InlineData("SpecificDateExchangeRateStrategy")]
        [InlineData("SpecificDateOrNextAvailableRateStrategy")]
        public void AdjustBalance_NoSufficientBalance_Throws(string strategy)
        {

        }

        //internal class ExchangeRateStrategyBaseMock : IExchangeRateStrategy
        //{
        //    public Task<decimal> Convert(decimal amount, string amountCurrencyCode, string currencyCodeToConvert, DateTime date)
        //    {
        //        return 5M;
        //    }
        //}

        //internal class SpecificDateExchangeRateStrategyMock : ExchangeRateStrategyBaseMock
        //{

        //}

    }
}
