using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrossCutting;
using Doppler.Currency.Dtos;
using Doppler.Currency.Logger;

namespace Doppler.Currency.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ILoggerAdapter<CurrencyService> _logger;
        private readonly IReadOnlyDictionary<CurrencyCode, CurrencyHandler> _currencyHandlers;

        public CurrencyService(
            ILoggerAdapter<CurrencyService> logger,
            IReadOnlyDictionary<CurrencyCode, CurrencyHandler> currencyHandlers) =>
            (_logger, _currencyHandlers) = (logger, currencyHandlers);

        public async Task<EntityOperationResult<Dtos.CurrencyDto>> GetCurrencyByCurrencyCodeAndDate(DateTime date, string currencyCode)
        {
            try
            {
                _logger.LogInformation("Service Getting currency code handler.");
                Enum.TryParse(typeof(CurrencyCode), currencyCode, true, out var result);
                _currencyHandlers.TryGetValue((CurrencyCode) result, out var handler);

                if (handler != null)
                    return await handler.Handle(date);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Invalid currency code.");
                var result2 = new EntityOperationResult<Dtos.CurrencyDto>();
                result2.AddError("Currency code invalid", $"Currency code invalid: {currencyCode}");
                return result2;
            }

            return null;
        }
    }
}
