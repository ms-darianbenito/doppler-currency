using System;
using Doppler.Currency.Enums;
using Doppler.Currency.Services;

namespace Doppler.Currency.Factory
{
    /// <summary>
    /// Class for the Currency factory
    /// </summary>
    public class CurrencyFactory : ICurrencyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a handler depending of currency code.
        /// </summary>
        /// <param name="currencyCode">The currency code</param>
        /// <returns>A handler</returns>
        public CurrencyHandler CreateHandler(CurrencyCodeEnum currencyCode)
        {
            switch (currencyCode)
            {
                case CurrencyCodeEnum.Ars:
                    return (BnaHandler)_serviceProvider.GetService(typeof(BnaHandler));
                case CurrencyCodeEnum.Mxn:
                    return (DofHandler)_serviceProvider.GetService(typeof(DofHandler));
                case CurrencyCodeEnum.Cop:
                    return (TrmHandler)_serviceProvider.GetService(typeof(TrmHandler));
                default:
                    throw new ArgumentException(nameof(currencyCode), $"The currencyCode '{currencyCode}' is not supported.");
            }
        }
    }
}
