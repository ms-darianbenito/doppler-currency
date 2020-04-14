using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrossCutting.SlackHooksService
{
    public class SlackHooksService : ISlackHooksService
    {
        private readonly JsonSerializerSettings _serializationSettings;
        private readonly SlackHookSettings _slackHookSettings;
        private readonly HttpClient _httpClient;

        public SlackHooksService(SlackHookSettings slackHookSettings, IHttpClientFactory httpClientFactory)
        {
            _slackHookSettings = slackHookSettings;
            _httpClient = httpClientFactory.CreateClient();

            _serializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new Iso8601TimeSpanConverter()
                }
            };
        }

        public async Task SendNotification(string message = null)
        {
            var payloadData = new
                {
                    text = !string.IsNullOrEmpty(message) ? message : _slackHookSettings.Text
                };

                var builder = new UriBuilder(_slackHookSettings.Url);

                var httpRequest = new HttpRequestMessage {RequestUri = builder.Uri, Method = new HttpMethod("POST")};

                var requestContent =
                    SafeJsonConvert.SerializeObject(JObject.FromObject(payloadData), _serializationSettings);
                httpRequest.Content = new StringContent(requestContent, Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
        }
    }
}
