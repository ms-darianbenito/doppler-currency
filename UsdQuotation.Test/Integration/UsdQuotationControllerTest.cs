using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace UsdQuotation.Test.Integration
{
    public class UsdQuotationControllerTest
    {
        private readonly HttpClient _client;

        public UsdQuotationControllerTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables();
                })
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Test]
        public async Task GetUsdToday_ShouldBeSendSlackNotificationError_WhenHtmlTitleIsNotCorrect()
        {
            // Act
            var response = await _client.GetAsync("UsdQuotation");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.IsNotEmpty(responseString);
        }
    }
}
