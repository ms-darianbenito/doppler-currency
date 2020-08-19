using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using CrossCutting.SlackHooksService;
using Doppler.Currency.Enums;
using Doppler.Currency.Services;
using Doppler.Currency.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Doppler.Currency.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public static class CreateSutCurrencyService
    {
        public static CurrencyService CreateSut(
            IHttpClientFactory httpClientFactory = null,
            HttpClientPoliciesSettings httpClientPoliciesSettings = null,
            IOptionsMonitor<CurrencySettings> currencySettings = null,
            ISlackHooksService slackHooksService = null,
            ILogger<CurrencyHandler> loggerHandler = null,
            ILogger<CurrencyService> loggerService = null)
        {
            var bnaHandler = new BnaHandler(
                httpClientFactory,
                httpClientPoliciesSettings,
                currencySettings,
                slackHooksService,
                loggerHandler ?? Mock.Of<ILogger<CurrencyHandler>>());

            var dofHandler = new DofHandler(
                httpClientFactory,
                httpClientPoliciesSettings,
                currencySettings,
                slackHooksService,
                loggerHandler ?? Mock.Of<ILogger<CurrencyHandler>>());

            var trmHandler= new TrmHandler(
                httpClientFactory,
                httpClientPoliciesSettings,
                currencySettings,
                slackHooksService,
                loggerHandler ?? Mock.Of<ILogger<CurrencyHandler>>());

            var handler = new Dictionary<CurrencyCodeEnum, CurrencyHandler>
            {
                { CurrencyCodeEnum.Ars, bnaHandler },
                { CurrencyCodeEnum.Mxn, dofHandler },
                { CurrencyCodeEnum.Cop, trmHandler }
            };

            return new CurrencyService(
                loggerService ?? Mock.Of<ILogger<CurrencyService>>(),
                handler);
        }
    }
}
