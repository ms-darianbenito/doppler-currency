using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrossCutting;
using Moq;
using Xunit;
using Doppler.Currency.Dtos;
using Doppler.Currency.Enums;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Doppler.Currency.Test.Integration
{
    public class CurrencyControllerTests: IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _testServer;
        private readonly HttpClient _client;

        public CurrencyControllerTests(TestServerFixture testServerFixture)
        {
            _testServer = testServerFixture;
            _client = _testServer.Client;
        }

        [Theory]
        [InlineData("2012-1-2", "ArS", "Peso Argentino", "ARS")]
        [InlineData("2012-01-02", "mxn", "Peso Mexicano", "MXN")]
        [InlineData("2012-01-2", "MXN", "Peso Mexicano", "MXN")]
        [InlineData("2012-1-02", "mXn", "Peso Mexicano", "MXM")]
        public async Task GetCurrency_ShouldBeHttpStatusCodeOk_WhenDateAndCurrencyCodeAreCorrectly(
            string dateTime,
            string currencyCode,
            string currencyName,
            string expectedCurrencyCode)
        {
            //Arrange
            var currency = new CurrencyDto
            {
                BuyValue = 10.3434M,
                SaleValue = 30.34M,
                Date = $"{DateTime.Parse(dateTime):yyyy-MM-dd}",
                CurrencyCode = expectedCurrencyCode,
                CurrencyName = currencyName
            };

            _testServer.CurrencyServiceMock.Setup(x => x.GetCurrencyByCurrencyCodeAndDate(
                    It.IsAny<DateTime>(),
                    It.IsAny<CurrencyCodeEnum>()))
                .ReturnsAsync(new EntityOperationResult<CurrencyDto>
                {
                    Entity = currency
                });

            // Act
            var response = await _client.GetAsync($"conversion/{currencyCode}/{dateTime}");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CurrencyDto>(responseString);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(responseString);
            Assert.Equal("2012-01-02", result.Date);
            Assert.Equal(30.34M, result.SaleValue);
            Assert.Equal(10.3434M, result.BuyValue);
            Assert.Equal(expectedCurrencyCode, result.CurrencyCode);
            Assert.Equal(currencyName, result.CurrencyName);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeBadRequest_WhenResponseReturnAnError()
        {
            //Arrange
            var result = new EntityOperationResult<CurrencyDto>();
            result.AddError("","");
            _testServer.CurrencyServiceMock.Setup(x => x.GetCurrencyByCurrencyCodeAndDate(
                    It.IsAny<DateTime>(),
                    It.IsAny<CurrencyCodeEnum>()))
                .ReturnsAsync(result);

            // Act
            var response = await _client.GetAsync("conversion/Ars/01-02-2012");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeNotFound_WhenUrlDoesNotHaveDateTime()
        {
            // Act
            var response = await _client.GetAsync("conversion/Ars");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeNotFound_WhenUrlDoesNotHaveCurrencyCode()
        {
            // Act
            var response = await _client.GetAsync("conversion/02-02-2020");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeBadRequest_WhenUrlDoesHaveInvalidCurrencyCode()
        {
            //Arrange
            const string currencyCode = "Test";
            var result = new EntityOperationResult<CurrencyDto>();
            result.AddError("Currency code invalid", $"Currency code invalid: {currencyCode}");
            _testServer.CurrencyServiceMock.Setup(x => x.GetCurrencyByCurrencyCodeAndDate(
                    It.IsAny<DateTime>(),
                    It.IsAny<CurrencyCodeEnum>()))
                .ReturnsAsync(result);

            // Act
            var response = await _client.GetAsync("conversion/TEST/02-02-2020");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeBadRequest_WhenDateIsMajorThatDateNow()
        {
            // Act
            var response = await _client.GetAsync("conversion/1/02-02-2550");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("02-223223-2020", "Ars")]
        [InlineData("02-aa-2020", "ArS")]
        [InlineData("3030-1-50", "ars")]
        [InlineData("02-2019-2020", "Mxn")]
        [InlineData("0202-220-2020", "mXN")]
        [InlineData("2020-20-02", "MXN")]
        [InlineData("31-2-2015", "MxN")]
        [InlineData("20-20-2020", "mXn")]
        [InlineData("20-20-2160", "mXn")]
        [InlineData("null", "mXn")]
        [InlineData("21-12-2012", "Ars")]
        [InlineData("21-2-2012", "Mxn")]
        public async Task GetCurrency_ShouldBeHttpStatusCodeBadRequest_WhenUrlDoesHaveInvalidDateTime(string dateTime, string currencyCode)
        {
            // Act
            var response = await _client.GetAsync($"conversion/{currencyCode}/{dateTime}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var text = await response.Content.ReadAsStringAsync();
            Assert.Contains($"The value '{dateTime}' is not valid", text);
        }

        [Theory]
        [InlineData(null, "Ars")]
        [InlineData(null, "mxn")]
        [InlineData("", "mXn")]
        [InlineData(" ", "ars")]
        public async Task GetCurrency_ShouldBeHttpStatusCodeNotFound_WhenUrlDoesHaveNullAndEmptyDateTime(string dateTime, string currencyCode)
        {
            // Act
            var response = await _client.GetAsync($"conversion/{currencyCode}/{dateTime}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("02/02/2020")]
        [InlineData("02\\02\\2020")]
        [InlineData("2020/02/02")]
        [InlineData("2020/02/0Z")]
        public async Task GetCurrency_ShouldBeHttpStatusCodeNotFound_WhenUrlDoesHaveInDateTimeInvalidCharacter(string dateTime)
        {
            // Act
            var response = await _client.GetAsync($"conversion/Arg/{dateTime}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("TEST")]
        [InlineData("ARGentina")]
        [InlineData("mexico")]
        [InlineData(" ")]
        public async Task GetCurrency_ShouldBeHttpStatusCodeNotFound_WhenUrlDoesHaveCurrencyCodeInvalid(
            string currencyCode)
        {
            // Act
            var response = await _client.GetAsync($"conversion/{currencyCode}/2020-2-7");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeUnauthorized_WhenRequestDoNotHaveToken()
        {
            // Arrange
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();
            var server = new TestServer(builder);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("conversion/1/2020-2-7");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrency_ShouldBeHttpStatusCodeUnauthorized_WhenRequestHaveExpiredToken()
        {
            // Arrange
            var builder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            var server = new TestServer(builder);
            var client = server.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://custom.domain.com/conversion/1/2020-2-7");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOjg4NDY5LCJ1bmlxdWVfbmFtZSI6ImFtb3NjaGluaUBtYWtpbmdzZW5zZS5jb20iLCJpc1N1IjpmYWxzZSwic3ViIjoiYW1vc2NoaW5pQG1ha2luZ3NlbnNlLmNvbSIsImN1c3RvbWVySWQiOiIxMzY3IiwiY2RoX2N1c3RvbWVySWQiOiIxMzY3Iiwicm9sZSI6IlVTRVIiLCJpYXQiOjE1OTQxNTUwMjYsImV4cCI6MTU5NDE1NjgyNn0.bv-ZHKulKMhBjcftiS-G_xa6MqPd8vmTJLCkitkSzz_lH6OblXnlLSjGAtoViT0yQun_IVqUggdfgY-Qv6cS_YeiYT-EqVLI1KFsFoWtZ7E1Yp5LZuVW70GskwZ7YbV7qlPrOOVBUbt6bD4LtwxudJmIenNBIgIVV-dCTl6vQNXRY65af7Ak1BG8IJxBaPhiFPniMIfNi_6my7NiHtL7Db2eeYgIxXf5_R-8BZFQ0CxWzNDTpdfaB48SnC7n6aEg9FQdOxcu8XX4qPBjGfnvCui2J9s8XgLfRtVQ27WwletL9XnGq79Dyp2PdNUsCcR2d4CMRxvzK1rO2jXSJ9Rf7w");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            var authenticateHeader = Assert.Single(response.Headers.WwwAuthenticate);
            if (authenticateHeader != null)
            {
                Assert.Equal("Bearer", authenticateHeader.Scheme);
                Assert.Contains("error=\"invalid_token\"", authenticateHeader.Parameter);
                Assert.Contains("error_description=\"The token expired at ", authenticateHeader.Parameter);
            }

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
