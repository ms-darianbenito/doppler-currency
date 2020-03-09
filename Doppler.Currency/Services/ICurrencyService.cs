using System;
using System.Threading.Tasks;
using CrossCutting;
using Doppler.Currency.Dtos;

namespace Doppler.Currency.Services
{
    public interface ICurrencyService
    {
        public Task<EntityOperationResult<Dtos.CurrencyDto>> GetCurrencyByCurrencyCodeAndDate(DateTime date, string currencyCode);
    }
}
