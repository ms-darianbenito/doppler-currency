﻿using System.Net.Http;
using CrossCutting.SlackHooksService;
using Doppler.Currency.Services;
using Microsoft.Extensions.Logging;
using Moq;
using UsdQuotation.Settings;

namespace UsdQuotation.Test.Integration
{
    public static class CreateSutBnaService
    {
        public static BnaService CreateSut(
            IHttpClientFactory httpClientFactory = null,
            HttpClientPoliciesSettings httpClientPoliciesSettings = null,
            BnaSettings bnaSettings = null,
            ISlackHooksService slackHooksService = null,
            ILogger<BnaService> logger = null)
        {
            return new BnaService(
                httpClientFactory ?? Mock.Of<IHttpClientFactory>(),
                httpClientPoliciesSettings ?? new HttpClientPoliciesSettings(),
                bnaSettings ?? new BnaSettings(),
                slackHooksService ?? Mock.Of<ISlackHooksService>(),
                logger ?? Mock.Of<ILogger<BnaService>>());
        }
    }
}
