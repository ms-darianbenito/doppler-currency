﻿using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CrossCutting.SlackHooksService
{
    [ExcludeFromCodeCoverage]
    public class DummySlackHooksService : ISlackHooksService
    {
        private readonly ILogger<ISlackHooksService> _slackHookLogger;
        public DummySlackHooksService(ILogger<ISlackHooksService> slackHookLogger)
        {
            _slackHookLogger = slackHookLogger;
        }

        public Task SendNotification(string message = null)
        {
            _slackHookLogger.LogInformation("Invalid Settings for Slack hook service.");
            return Task.CompletedTask;
        }
    }
}
