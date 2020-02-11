using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CrossCutting.SlackHooksService;
using Microsoft.Extensions.Logging;
using UsdQuotation.Dtos;
using UsdQuotation.Settings;

namespace UsdQuotation.Services
{
    public class BnaService : IBnaService
    {
        private readonly HttpClient _httpClient;
        private readonly BnaSettings _bnaSettings;
        private readonly ISlackHooksService _slackHooksService;
        private readonly ILogger<BnaService> _logger;

        public BnaService(IHttpClientFactory httpClientFactory,
            HttpClientPoliciesSettings bnaClientPoliciesSettings,
            BnaSettings bnaSettings,
            ISlackHooksService slackHooksService,
            ILogger<BnaService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(bnaClientPoliciesSettings.ClientName);
            _bnaSettings = bnaSettings;
            _slackHooksService = slackHooksService;
            _logger = logger;
        }

        public async Task<Usd> GetUsdToday()
        {
            // Construct URL
            var dateUrl = System.Web.HttpUtility.UrlEncode($"{DateTime.Now:dd/MM/yyyy}");
            var uri = new Uri(_bnaSettings.EndPoint + "&fecha=" + dateUrl);

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = uri, 
                Method = new HttpMethod("GET")
            };

            _logger.LogInformation("Sending request to Bna server.");
            var httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

            _logger.LogInformation("Getting Html content of the Bna.");
            var htmlPage = await httpResponse.Content.ReadAsStringAsync();

            return await GetDataFromHtmlAsync(htmlPage);
        }

        private async Task<Usd> GetDataFromHtmlAsync(string htmlPage)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlPage);

            if (document.GetElementsByClassName("sinResultados").Any())
            {
                _logger.LogError("Error getting HTML, currently does not exist quotation USD.");
                await _slackHooksService.SendNotification(_httpClient, _bnaSettings.NoQuotation);
                return null;
            }

            var titleValidation = document.GetElementsByTagName("tr").ElementAtOrDefault(1);
            if (titleValidation == null)
            {
                _logger.LogError($"Error getting HTML, title is not valid, please check HTML: {htmlPage}");
                await _slackHooksService.SendNotification(_httpClient);
                return null;
            }

            var titleText = titleValidation.GetElementsByTagName("td").ElementAtOrDefault(0);
            if (titleText != null && !titleText.InnerHtml.Equals(_bnaSettings.ValidationHtml))
            {
                _logger.LogError($"Error getting HTML, currently does not exist quotation USD: {htmlPage}");
                await _slackHooksService.SendNotification(_httpClient);
                return null;
            }

            var usdToday = document.GetElementsByTagName("tr").LastOrDefault();

            if (usdToday == null)
            {
                _logger.LogError($"Error getting HTML, please check HTML: {htmlPage}");
                await _slackHooksService.SendNotification(_httpClient);
                return null;
            }

            var buy = usdToday.GetElementsByTagName("td").ElementAtOrDefault(1);
            var sale = usdToday.GetElementsByTagName("td").ElementAtOrDefault(2);
            var date = usdToday.GetElementsByTagName("td").ElementAtOrDefault(3);

            if (buy != null && sale != null && date != null)
            {
                return new Usd
                {
                    Date = date.InnerHtml,
                    SaleValue = sale.InnerHtml,
                    BuyValue = buy.InnerHtml
                };
            }

            _logger.LogError($"Error getting HTML, please check HTML: {htmlPage}");
            await _slackHooksService.SendNotification(_httpClient);
            return null;
        }
    }
}
