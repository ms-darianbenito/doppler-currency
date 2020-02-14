using System;
using System.Threading.Tasks;
using UsdQuotation.Dtos;

namespace UsdQuotation.Services
{
    public interface IBnaService
    {
        public Task<Usd> GetUsdToday(DateTime? date);
    }
}