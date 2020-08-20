using System.Collections.Generic;
using Doppler.Currency.Enums;
using Doppler.Currency.Services;

namespace Doppler.Currency.Handlers
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IReadOnlyDictionary<CurrencyCodeEnum, CurrencyHandler> _currencyHandlers;

        public HandlerFactory(IReadOnlyDictionary<CurrencyCodeEnum, CurrencyHandler> currencyHandlers)
        {
            _currencyHandlers = currencyHandlers;
        }

        public CurrencyHandler GetCurrencyHandler(CurrencyCodeEnum currencyCode)
        {
            _currencyHandlers.TryGetValue(currencyCode, out var handler);
            
            return handler;
        }
    }
}
