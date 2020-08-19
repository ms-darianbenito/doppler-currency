using System;
using System.Net.Http;
using System.Threading.Tasks;
using CrossCutting;
using CrossCutting.SlackHooksService;
using Doppler.Currency.Dtos;
using Doppler.Currency.Enums;
using Doppler.Currency.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Doppler.Currency.Services
{
    public class TrmHandler : CurrencyHandler
    {
        public TrmHandler(
            IHttpClientFactory httpClientFactory, 
            HttpClientPoliciesSettings httpClientPoliciesSettings,
            IOptionsMonitor<CurrencySettings> serviceSettings, 
            ISlackHooksService slackHooksService, 
            ILogger<CurrencyHandler> logger) : base(httpClientFactory, httpClientPoliciesSettings, serviceSettings.Get("TrmService"), slackHooksService, logger) { }

        public override async Task<EntityOperationResult<CurrencyDto>> Handle(DateTime date)
        {
            // Construct URL
            Logger.LogInformation("building url to get html data.");
            var dateUrl = System.Web.HttpUtility.UrlEncode($"{date:yyyy-MM-dd}");
            var uri = new Uri($"{ServiceSettings.Url}{dateUrl}");

            Logger.LogInformation("Building http request with url {uri}", uri);
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = new HttpMethod("GET")
            };

            Logger.LogInformation("Sending request to Bna server.");
            var client = HttpClientFactory.CreateClient();
            var httpResponse = await client.SendAsync(httpRequest).ConfigureAwait(false);

            Logger.LogInformation("Getting Html content of the Bna.");
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();

            return await GetDataFromHtmlAsync(jsonContent, date);
        }

        private async Task<EntityOperationResult<CurrencyDto>> GetDataFromHtmlAsync(string jsonContent, DateTime date)
        {
            var result = new EntityOperationResult<CurrencyDto>();
            dynamic data = JsonConvert.DeserializeObject(jsonContent);

            if (data != null)
            {
                result.Entity = new CurrencyDto
                {
                    Date = $"{date.ToUniversalTime():yyyy-MM-dd}",
                    SaleValue = data[0].valor,
                    CurrencyName = ServiceSettings.CurrencyName,
                    CurrencyCode = ServiceSettings.CurrencyCode.ToUpper()
                };
            }
            else
            {
                await SendSlackNotification(jsonContent, date, CurrencyCodeEnum.Cop);
                result.AddError("Html Error COP currency", "Error getting currency.");
            }

            return result;
        }
    }
}
