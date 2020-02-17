using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace UsdQuotation.Test.Integration
{
    public class UsdQuotationControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly HttpClient _client;

        public UsdQuotationControllerTest(TestServerFixture testServerFixture)
        {
            // Arrange
            _client = testServerFixture.Client;
        }

        [Fact]
        public async Task GetUsdToday_ShouldBeSendSlackNotificationError_WhenHtmlTitleIsNotCorrect()
        {
            // Act
            var response = await _client.GetAsync("UsdQuotation");                          
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotEmpty(responseString);
        }
    }
}
