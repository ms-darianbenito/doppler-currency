using Doppler.Currency.Enums;
using Doppler.Currency.Services;

namespace Doppler.Currency.Factory
{
    /// <summary>
    /// Interface for the Currency factory
    /// </summary>
    public interface ICurrencyFactory
    {
        /// <summary>
        /// Create a handler depending of currency code.
        /// </summary>
        /// <param name="currencyCode">The currency code</param>
        /// <returns>A handler</returns>
        CurrencyHandler CreateHandler(CurrencyCodeEnum currencyCode);
    }
}
