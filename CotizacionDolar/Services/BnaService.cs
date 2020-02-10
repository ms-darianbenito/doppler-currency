using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using CrossCutting.SlackHooksService;
using UsdQuotation.Dtos;
using UsdQuotation.Settings;

namespace UsdQuotation.Services
{
    public class BnaService : IBnaService
    {
        private readonly HttpClient _httpClient;
        private readonly BnaSettings _bnaSettings;
        private readonly ISlackHooksService _slackHooksService;

        public BnaService(IHttpClientFactory httpClientFactory,
            HttpClientPoliciesSettings bnaClientPoliciesSettings,
            BnaSettings bnaSettings,
            ISlackHooksService slackHooksService)
        {
            _httpClient = httpClientFactory.CreateClient(bnaClientPoliciesSettings.ClientName);
            _bnaSettings = bnaSettings;
            _slackHooksService = slackHooksService;
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

            var httpResponse = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

            var htmlPage = await httpResponse.Content.ReadAsStringAsync();

            return await GetDataFromHtmlAsync(htmlPage);
        }

        private async Task<Usd> GetDataFromHtmlAsync(string htmlPage)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlPage);

            if (document.GetElementsByClassName("sinResultados").Any())
            {
                await _slackHooksService.SendNotification(_httpClient, _bnaSettings.NoQuotation);
                return null;
            }

            var titleValidation = document.GetElementsByTagName("tr").ElementAtOrDefault(1);
            if (titleValidation == null)
            {
                await _slackHooksService.SendNotification(_httpClient);
                return null;
            }

            var titleText = titleValidation.GetElementsByTagName("td").ElementAtOrDefault(0);
            if (titleText != null && !titleText.InnerHtml.Equals(_bnaSettings.ValidationHtml))
            {
                await _slackHooksService.SendNotification(_httpClient);
                return null;
            }

            var usdToday = document.GetElementsByTagName("tr").LastOrDefault();

            if (usdToday == null)
            {
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

            await _slackHooksService.SendNotification(_httpClient);
            return null;
        }
    }
}
