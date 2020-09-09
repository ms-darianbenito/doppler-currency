using System.Diagnostics.CodeAnalysis;
using Doppler.Currency.Factory;
using Doppler.Currency.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Doppler.Currency.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public static class CreateSutCurrencyService
    {
        public static CurrencyService CreateSut(
            ILogger<CurrencyService> loggerService = null,
            ICurrencyFactory currencyFactory = null)
        {
            return new CurrencyService(
                loggerService ?? Mock.Of<ILogger<CurrencyService>>(),
                currencyFactory ?? Mock.Of<ICurrencyFactory>());
        }
    }
}
