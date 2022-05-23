using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EuropeanCentralBank.Tests
{
    public class Client_Implementation_Tests
    {
        private readonly Mock<IOptions<EuropeanCentralBankSettings>> optionSettings;

        private EuropeanCentralBankClient europeanCentralBankClient;
        public Client_Implementation_Tests()
        {
            var settings = new EuropeanCentralBankSettings() { RatesEndpoint = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml" };
            optionSettings = new Mock<IOptions<EuropeanCentralBankSettings>>();
            optionSettings.Setup(x => x.Value).Returns(settings);
        }

        [Fact]
        public void GetRates_UnableToGetRatesFromECB_Throws()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            europeanCentralBankClient = new EuropeanCentralBankClient(new HttpClient(mockMessageHandler.Object), optionSettings.Object);

            var ex = Assert.ThrowsAsync<Exception>(() => europeanCentralBankClient.GetRates());

            Assert.Equal("Unable to retrieve rates from ECB.", ex.Result.Message);
        }

        [Fact]
        public void GetRates_UnableToParseXML_Throws()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                 .Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"<gesmeCube currency=""ZAR"" rate=""16.7131""/></Cube></Cube></gesmes:Envelope>")
                });

            europeanCentralBankClient = new EuropeanCentralBankClient(new HttpClient(mockMessageHandler.Object), optionSettings.Object);

            var ex = Assert.ThrowsAsync<Exception>(() => europeanCentralBankClient.GetRates());

            Assert.Equal("Unable to parse ECB response.", ex.Result.Message);
        }

        [Fact]
        public void GetRates_MissingDate_Throws()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"<gesmes:Envelope xmlns:gesmes=""http://www.gesmes.org/xml/2002-08-01"" 
                            xmlns=""http://www.ecb.int/vocabulary/2002-08-01/eurofxref""><gesmes:subject>Reference rates</gesmes:subject><gesmes:Sender>
                            <gesmes:name>European Central Bank</gesmes:name></gesmes:Sender>
                            <Cube><Cube><Cube currency=""USD"" rate=""1.0577""/><Cube currency=""JPY"" rate=""135.34""/>
                            <Cube currency=""BGN"" rate=""1.9558""/><Cube currency=""CZK"" rate=""24.670""/><Cube currency=""DKK"" rate=""7.4424""/>
                            <Cube currency=""GBP"" rate=""0.84820""/><Cube currency=""HUF"" rate=""382.93""/><Cube currency=""PLN"" rate=""4.6365""/>
                            <Cube currency=""RON"" rate=""4.9477""/><Cube currency=""SEK"" rate=""10.4915""/><Cube currency=""CHF"" rate=""1.0280""/>
                            <Cube currency=""ISK"" rate=""138.50""/><Cube currency=""NOK"" rate=""10.2620""/><Cube currency=""HRK"" rate=""7.5335""/>
                            <Cube currency=""TRY"" rate=""16.8201""/><Cube currency=""AUD"" rate=""1.4980""/><Cube currency=""BRL"" rate=""5.1989""/>
                            <Cube currency=""CAD"" rate=""1.3526""/><Cube currency=""CNY"" rate=""7.0638""/><Cube currency=""HKD"" rate=""8.2999""/>
                            <Cube currency=""IDR"" rate=""15501.99""/><Cube currency=""ILS"" rate=""3.5330""/><Cube currency=""INR"" rate=""82.1617""/>
                            <Cube currency=""KRW"" rate=""1340.58""/><Cube currency=""MXN"" rate=""21.0314""/><Cube currency=""MYR"" rate=""4.6422""/>
                            <Cube currency=""NZD"" rate=""1.6518""/><Cube currency=""PHP"" rate=""55.181""/><Cube currency=""SGD"" rate=""1.4588""/>
                            <Cube currency=""THB"" rate=""36.284""/><Cube currency=""ZAR"" rate=""16.7131""/></Cube></Cube></gesmes:Envelope>")
                });

            europeanCentralBankClient = new EuropeanCentralBankClient(new HttpClient(mockMessageHandler.Object), optionSettings.Object);

            var ex = Assert.ThrowsAsync<Exception>(() => europeanCentralBankClient.GetRates());

            Assert.Equal("ECB response missing date.", ex.Result.Message);
        }

        [Fact]
        public void GetRates_MissingRates_Throws()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"<gesmes:Envelope xmlns:gesmes=""http://www.gesmes.org/xml/2002-08-01"" 
                            xmlns=""http://www.ecb.int/vocabulary/2002-08-01/eurofxref""><gesmes:subject>Reference rates</gesmes:subject><gesmes:Sender>
                            <gesmes:name>European Central Bank</gesmes:name></gesmes:Sender>
                            <Cube><Cube time=""2022-05-20""></Cube></Cube></gesmes:Envelope>")
                });

            europeanCentralBankClient = new EuropeanCentralBankClient(new HttpClient(mockMessageHandler.Object), optionSettings.Object);

            var ex = Assert.ThrowsAsync<Exception>(() => europeanCentralBankClient.GetRates());

            Assert.Equal("ECB response missing rates.", ex.Result.Message);
        }

        [Fact]
        public async Task GetRates_AllOk_Success()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"<gesmes:Envelope xmlns:gesmes=""http://www.gesmes.org/xml/2002-08-01"" 
                            xmlns=""http://www.ecb.int/vocabulary/2002-08-01/eurofxref""><gesmes:subject>Reference rates</gesmes:subject><gesmes:Sender>
                            <gesmes:name>European Central Bank</gesmes:name></gesmes:Sender>
                            <Cube><Cube time=""2022-05-20""><Cube currency=""USD"" rate=""1.0577""/><Cube currency=""JPY"" rate=""135.34""/>
                            <Cube currency=""BGN"" rate=""1.9558""/><Cube currency=""CZK"" rate=""24.670""/><Cube currency=""DKK"" rate=""7.4424""/>
                            <Cube currency=""GBP"" rate=""0.84820""/><Cube currency=""HUF"" rate=""382.93""/><Cube currency=""PLN"" rate=""4.6365""/>
                            <Cube currency=""RON"" rate=""4.9477""/><Cube currency=""SEK"" rate=""10.4915""/><Cube currency=""CHF"" rate=""1.0280""/>
                            <Cube currency=""ISK"" rate=""138.50""/><Cube currency=""NOK"" rate=""10.2620""/><Cube currency=""HRK"" rate=""7.5335""/>
                            <Cube currency=""TRY"" rate=""16.8201""/><Cube currency=""AUD"" rate=""1.4980""/><Cube currency=""BRL"" rate=""5.1989""/>
                            <Cube currency=""CAD"" rate=""1.3526""/><Cube currency=""CNY"" rate=""7.0638""/><Cube currency=""HKD"" rate=""8.2999""/>
                            <Cube currency=""IDR"" rate=""15501.99""/><Cube currency=""ILS"" rate=""3.5330""/><Cube currency=""INR"" rate=""82.1617""/>
                            <Cube currency=""KRW"" rate=""1340.58""/><Cube currency=""MXN"" rate=""21.0314""/><Cube currency=""MYR"" rate=""4.6422""/>
                            <Cube currency=""NZD"" rate=""1.6518""/><Cube currency=""PHP"" rate=""55.181""/><Cube currency=""SGD"" rate=""1.4588""/>
                            <Cube currency=""THB"" rate=""36.284""/><Cube currency=""ZAR"" rate=""16.7131""/></Cube></Cube></gesmes:Envelope>")
                });

            europeanCentralBankClient = new EuropeanCentralBankClient(new HttpClient(mockMessageHandler.Object), optionSettings.Object);

            var rates = await europeanCentralBankClient.GetRates();
            Assert.NotNull(rates);
            Assert.Equal(new DateTime(2022, 05, 20), rates.Date.Date);
            Assert.Equal(31, rates.Rates.Count);
        }
    }
}
