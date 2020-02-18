using System;
using System.Net.Http;
using System.Net.Http.Headers;
using CrossCutting.SlackHooksService;
using Doppler.Currency;
using Doppler.Currency.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace UsdQuotation.Test.Integration
{
    public class TestServerFixture : IDisposable
    {
        public TestServer Server { get; }

        public HttpClient Client { get; }

        public Mock<IBnaService> BnaServiceMock;
        public Mock<ISlackHooksService> SlackHookServiceMock;

        public TestServerFixture()
        {
            BnaServiceMock = new Mock<IBnaService>();
            SlackHookServiceMock = new Mock<ISlackHooksService>();

            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton(BnaServiceMock.Object);
                    services.AddSingleton(SlackHookServiceMock.Object);
                });

            Server = new TestServer(builder);
            Client = Server.CreateClient();

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            Server.Dispose();
            Client.Dispose();
        }
    }
}