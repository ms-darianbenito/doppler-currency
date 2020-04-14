using System.Net.Http;
using CrossCutting;
using CrossCutting.SlackHooksService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Doppler.Currency
{
    public static class SlackHookServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackHook(this IServiceCollection services)
        {
            services.AddSingleton<ISlackHooksService>(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var section = configuration.GetSection("SlackHook");

                if (!section.Exists()) 
                    return new DummySlackHooksService(provider.GetService<ILogger<DummySlackHooksService>>());

                var slackHookSettings = new SlackHookSettings();
                section.Bind(slackHookSettings);

                return new SlackHooksService(slackHookSettings, provider.GetService<IHttpClientFactory>());

            });

            return services;
        }
    }
}
