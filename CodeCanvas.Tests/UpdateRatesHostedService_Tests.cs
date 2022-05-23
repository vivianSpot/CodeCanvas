using CodeCanvas.Entities;
using CodeCanvas.HostedServices;
using CodeCanvas.Repositories;
using EuropeanCentralBank;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeCanvas.Tests
{
    public class UpdateRatesHostedService_Tests
    {
        private UpdateRatesHostedService updateRatesHostedService;
        private readonly Mock<IEuropeanCentralBankClient> _europeanCentralBankClient;
        private readonly Mock<ICurrencyRateRepository> _repository;
        private readonly Mock<ILogger<UpdateRatesHostedService>> logger;

        public UpdateRatesHostedService_Tests()
        {
            _europeanCentralBankClient = new Mock<IEuropeanCentralBankClient>();
            _repository = new Mock<ICurrencyRateRepository>();
            logger = new Mock<ILogger<UpdateRatesHostedService>>();
        }

        [Fact]
        public async Task UpdateRates_NoRepositoryCall_Success()
        {
            var currentDate = DateTime.UtcNow;
            var rates = new List<CurrencyRate>()
            {
                new CurrencyRate("USD", 0.1M),
                new CurrencyRate("GBP", 0.2M)
            };

            _europeanCentralBankClient.Setup(x => x.GetRates())
                .ReturnsAsync(new RatesResponse(currentDate, rates.AsReadOnly()));

            _repository.Setup(x => x.GetAllByDate(It.IsAny<DateTime>()))
                .Returns(new List<CurrencyRateEntity>() {
                    new CurrencyRateEntity(1, "USD", 0.1M, currentDate, currentDate),
                    new CurrencyRateEntity(2, "GBP", 0.2M, currentDate, currentDate)
                });

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyRateRepository)))
                .Returns(_repository.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            updateRatesHostedService = new UpdateRatesHostedService(_europeanCentralBankClient.Object, serviceScopeFactory.Object, logger.Object);

            await updateRatesHostedService.StartAsync(CancellationToken.None);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await updateRatesHostedService.StopAsync(CancellationToken.None);

            _repository.Verify(x => x.Update(It.IsAny<CurrencyRateEntity>()), Times.Never);
            _repository.Verify(x => x.InsertCurrencyRate(It.IsAny<CurrencyRateEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRates_NoPersistedRatesInsertAll_Success()
        {
            var currentDate = DateTime.UtcNow;
            var rates = new List<CurrencyRate>()
            {
                new CurrencyRate("USD", 0.1M),
                new CurrencyRate("GBP", 0.2M)
            };

            _europeanCentralBankClient.Setup(x => x.GetRates())
                .ReturnsAsync(new RatesResponse(currentDate, rates));

            _repository.Setup(x => x.GetAllByDate(It.IsAny<DateTime>()))
                .Returns(new List<CurrencyRateEntity>());

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyRateRepository)))
                .Returns(_repository.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            updateRatesHostedService = new UpdateRatesHostedService(_europeanCentralBankClient.Object, serviceScopeFactory.Object, logger.Object);

            await updateRatesHostedService.StartAsync(CancellationToken.None);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await updateRatesHostedService.StopAsync(CancellationToken.None);

            _repository.Verify(x => x.Update(It.IsAny<CurrencyRateEntity>()), Times.Never);
            _repository.Verify(x => x.InsertCurrencyRate(It.IsAny<CurrencyRateEntity>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateRates_UpdatePersistedRates_Success()
        {
            var currentDate = DateTime.UtcNow;
            var rates = new List<CurrencyRate>()
            {
                new CurrencyRate("USD", 0.1M),
                new CurrencyRate("GBP", 0.2M)
            };

            _europeanCentralBankClient.Setup(x => x.GetRates())
                .ReturnsAsync(new RatesResponse(currentDate, rates.AsReadOnly()));

            _repository.Setup(x => x.GetAllByDate(It.IsAny<DateTime>()))
                .Returns(new List<CurrencyRateEntity>() { 
                    new CurrencyRateEntity(1, "USD", 0.3M, currentDate, currentDate)
                });

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ICurrencyRateRepository)))
                .Returns(_repository.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            updateRatesHostedService = new UpdateRatesHostedService(_europeanCentralBankClient.Object, serviceScopeFactory.Object, logger.Object);

            await updateRatesHostedService.StartAsync(CancellationToken.None);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await updateRatesHostedService.StopAsync(CancellationToken.None);

            _repository.Verify(x => x.Update(It.IsAny<CurrencyRateEntity>()), Times.Once);
            _repository.Verify(x => x.InsertCurrencyRate(It.IsAny<CurrencyRateEntity>()), Times.Once);
        }
    }
}
