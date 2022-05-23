using CodeCanvas.Entities;
using CodeCanvas.Repositories;
using CodeCanvas.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CodeCanvas.Tests
{
    public class WalletAdjustmentService_Tests
    {
        private readonly Mock<ICurrencyRateRepository> _currencyRateRepository;
        private readonly Mock<IWalletRepository> _walletRepository;
        private WalletAdjustmentService walletAdjustmentService;

        public WalletAdjustmentService_Tests()
        {
            _currencyRateRepository = new Mock<ICurrencyRateRepository>();
            _walletRepository = new Mock<IWalletRepository>();
        }

        [Fact]
        public void AdjustBalance_NoWallet_Throws()
        {
            walletAdjustmentService = new WalletAdjustmentService(_currencyRateRepository.Object, _walletRepository.Object);
            var ex = Assert.ThrowsAsync<Exception>(() => walletAdjustmentService.AdjustBalance("SpecificDateExchangeRateStrategy", 0, "USD", 5));
            
            Assert.Equal("No wallet found for id 0", ex.Result.Message);
            _walletRepository.Verify(x => x.Update(It.IsAny<WalletEntity>()), Times.Never);
        }

        [Theory]
        [InlineData("SpecificDateExchangeRateStrategy")]
        [InlineData("SpecificDateOrNextAvailableRateStrategy")]
        public void AdjustBalance_Convert_Throws(string strategy)
        {
            var currentDate = DateTime.UtcNow;
            _walletRepository.Setup(x => x.GetWalletByIdAsync(1))
                .ReturnsAsync(new WalletEntity(1, "GBP", 2, currentDate, currentDate));

            _currencyRateRepository.Setup(x => x.GetCurrencyRateByDateAndCurrencyCodeAsync(It.IsAny<DateTime>(), "USD"))
               .ReturnsAsync(new CurrencyRateEntity("USD", 1.2M, DateTime.UtcNow.AddDays(-2)));

            walletAdjustmentService = new WalletAdjustmentService(_currencyRateRepository.Object, _walletRepository.Object);
            var ex = Assert.ThrowsAsync<Exception>(() => walletAdjustmentService.AdjustBalance(strategy, 1, "USD", 5));

            Assert.NotNull(ex.Result.Message);
            Assert.Contains("GBP", ex.Result.Message);
            _walletRepository.Verify(x => x.Update(It.IsAny<WalletEntity>()), Times.Never);
        }

        [Theory]
        [InlineData("SpecificDateExchangeRateStrategy")]
        [InlineData("SpecificDateOrNextAvailableRateStrategy")]
        public void AdjustBalance_NoSufficientBalance_Throws(string strategy)
        {
            _walletRepository.Setup(x => x.GetWalletByIdAsync(1))
                .ReturnsAsync(new WalletEntity(1, "GBP", -30, DateTime.UtcNow, DateTime.UtcNow));

            _currencyRateRepository.Setup(x => x.GetCurrencyRateByDateAndCurrencyCodeAsync(It.IsAny<DateTime>(), "GBP"))
                .ReturnsAsync(new CurrencyRateEntity("GBP", 0.8M, DateTime.UtcNow.AddDays(-2)));

            _currencyRateRepository.Setup(x => x.GetCurrencyRateByDateAndCurrencyCodeAsync(It.IsAny<DateTime>(), "USD"))
               .ReturnsAsync(new CurrencyRateEntity("USD", 1.2M, DateTime.UtcNow.AddDays(-2)));

            walletAdjustmentService = new WalletAdjustmentService(_currencyRateRepository.Object, _walletRepository.Object);
            var ex = Assert.ThrowsAsync<NoSufficientBalanceException>(() => walletAdjustmentService.AdjustBalance(strategy, 1, "USD", 5));

            Assert.NotNull(ex.Result.Message);
            _walletRepository.Verify(x => x.Update(It.IsAny<WalletEntity>()), Times.Never);
        }

        [Theory]
        [InlineData("SpecificDateExchangeRateStrategy")]
        [InlineData("SpecificDateOrNextAvailableRateStrategy")]
        public async Task AdjustBalance_AllOk_Success(string strategy)
        {
            _walletRepository.Setup(x => x.GetWalletByIdAsync(1))
                .ReturnsAsync(new WalletEntity(1, "GBP", 30, DateTime.UtcNow, DateTime.UtcNow));

            _currencyRateRepository.Setup(x => x.GetCurrencyRateByDateAndCurrencyCodeAsync(It.IsAny<DateTime>(), "GBP"))
                .ReturnsAsync(new CurrencyRateEntity("GBP", 0.6M, DateTime.UtcNow.AddDays(-2)));

            _currencyRateRepository.Setup(x => x.GetCurrencyRateByDateAndCurrencyCodeAsync(It.IsAny<DateTime>(), "USD"))
               .ReturnsAsync(new CurrencyRateEntity("USD", 1.2M, DateTime.UtcNow.AddDays(-2)));

            walletAdjustmentService = new WalletAdjustmentService(_currencyRateRepository.Object, _walletRepository.Object);
            var result =  await walletAdjustmentService.AdjustBalance(strategy, 1, "USD", 5);

            Assert.Equal(32.5M, result);
            _walletRepository.Verify(x => x.SaveAllAsync(), Times.Once);
        }

    }
}
