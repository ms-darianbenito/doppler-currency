using System;
using System.Threading.Tasks;
using CrossCutting;
using Doppler.Currency.Dtos;
using Doppler.Currency.Enums;
using Doppler.Currency.Factory;
using Microsoft.Extensions.Logging;

namespace Doppler.Currency.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ILogger<CurrencyService> _logger;
        private readonly ICurrencyFactory _currencyFactory;

        public CurrencyService(
            ILogger<CurrencyService> logger,
            ICurrencyFactory currencyFactory) =>
            (_logger, _currencyFactory) = (logger, currencyFactory);

        public async Task<EntityOperationResult<CurrencyDto>> GetCurrencyByCurrencyCodeAndDate(
            DateTime date,
            CurrencyCodeEnum currencyCode)
        {
            var result = new EntityOperationResult<CurrencyDto>();
            try
            {
                _logger.LogInformation("Service Getting currency code handler.");

                var currencyHandler = _currencyFactory.CreateHandler(currencyCode);
                return await currencyHandler.Handle(date);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Error to get currency.");
                result.AddError("Error to get currency", $"Please see log, currency code : {currencyCode} and date  {date}.");
                return result;
            }
        }
    }
}
