using System;
using System.Threading.Tasks;
using CrossCutting;
using Doppler.Currency.Dtos;

namespace Doppler.Currency.Services
{
    public interface IBnaService
    {
        public Task<EntityOperationResult<UsdCurrency>> GetUsdToday(DateTime? date);
    }
}