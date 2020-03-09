using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Doppler.Currency.Services
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CurrencyCode
    {
        [JsonProperty("ars")]
        Ars = 1,
        [JsonProperty("mxn")]
        Mxn = 2
    }
}
