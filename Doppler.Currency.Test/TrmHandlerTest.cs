using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CrossCutting.SlackHooksService;
using Doppler.Currency.Enums;
using Doppler.Currency.Services;
using Doppler.Currency.Settings;
using Doppler.Currency.Test.Integration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Doppler.Currency.Test
{
    public class TrmHandlerTest
    {
        private readonly Mock<IOptionsMonitor<CurrencySettings>> _mockUsdCurrencySettings;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public TrmHandlerTest()
        {
            _mockUsdCurrencySettings = new Mock<IOptionsMonitor<CurrencySettings>>();
            _mockUsdCurrencySettings.Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new CurrencySettings
                {
                    Url = "https://www.datos.gov.co/resource/ceyp-9c7c.json?VIGENCIAHASTA=",
                    NoCurrency = "",
                    CurrencyName = "Peso Colombiano",
                    CurrencyCode = "COP",
                });
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task GetCurrency_ShouldBeReturnCurrencyOk_WhenTrmHasInformation()
        {
            var dateTime = new DateTime(2020, 02, 05);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"valor\":\"3783.15\",\"vigenciadesde\":\"2020-02-05T00: 00:00.000\",\"vigenciahasta\":\"2020-02-05T00: 00:00.000\"}]")
                });

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);

            var service = CreateSutCurrencyService.CreateSut(
                _httpClientFactoryMock.Object,
                new HttpClientPoliciesSettings
                {
                    ClientName = "test"
                },
                _mockUsdCurrencySettings.Object,
                Mock.Of<ISlackHooksService>(),
                Mock.Of<ILogger<CurrencyHandler>>());

            var result = await service.GetCurrencyByCurrencyCodeAndDate(dateTime, CurrencyCodeEnum.Cop);

            Assert.Equal("2020-02-05", result.Entity.Date);
            Assert.Equal("Peso Colombiano", result.Entity.CurrencyName);
            Assert.Equal("COP", result.Entity.CurrencyCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeSendSlackNotification_WhenTrmDoesNotHaveInformation()
        {
            var dateTime = new DateTime(2020, 02, 05);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);

            var slackHookServiceMock = new Mock<ISlackHooksService>();
            var service = CreateSutCurrencyService.CreateSut(
                _httpClientFactoryMock.Object,
                new HttpClientPoliciesSettings
                {
                    ClientName = "test"
                },
                _mockUsdCurrencySettings.Object,
                slackHookServiceMock.Object,
                Mock.Of<ILogger<CurrencyHandler>>());

            var result = await service.GetCurrencyByCurrencyCodeAndDate(dateTime, CurrencyCodeEnum.Cop);

           Assert.Null(result.Entity);
           Assert.Equal(1, result.Errors.Count);
           slackHookServiceMock.Verify(x => x.SendNotification(It.IsAny<string>()), Times.Once);
        }
    }
}
