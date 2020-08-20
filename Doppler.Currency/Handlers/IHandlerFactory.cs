using Doppler.Currency.Enums;
using Doppler.Currency.Services;

namespace Doppler.Currency.Handlers
{
    public interface IHandlerFactory
    {
        public CurrencyHandler GetCurrencyHandler(CurrencyCodeEnum currencyCode);
    }
}