using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Doppler.Currency.Dtos
{
    public class CurrencyDto
    {
        public string Date { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? SaleValue { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? BuyValue { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public bool CotizationAvailable => SaleValue.HasValue && SaleValue != 0;

    }
}
