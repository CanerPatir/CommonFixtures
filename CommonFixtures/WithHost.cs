using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommonFixtures
{
    public abstract class WithHost : WithIoC
    {
        private class TestHostFactory : IDisposable
        {
            private readonly IHost _host;

            public TestServer Server { get; }

            public TestHostFactory(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
            {
                if (configureServices == null) throw new ArgumentNullException(nameof(configureServices));
                if (configure == null) throw new ArgumentNullException(nameof(configure));
                _host = CreateHostBuilder(configureServices, configure).Build();
                _host.Start();
                Server = (TestServer) _host.Services.GetRequiredService<IServer>();
            }

            private static IHostBuilder CreateHostBuilder(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
            {
                return Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration(builder =>
                    {
                        builder.Sources.Clear();
                        builder.AddEnvironmentVariables();
                    })
                    .UseEnvironment(Environments.Development)
                    .ConfigureWebHost(webBuilder =>
                    {
                        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                        webBuilder.ConfigureServices(configureServices);
                        webBuilder.Configure(configure);
                        webBuilder.UseTestServer();
                    });
            }

            public void Dispose()
            {
                _host?.Dispose();
                Server?.Dispose();
            }
        }

        private readonly TestHostFactory _testHostFactory;
        protected TestServer Server { get; }

        protected WithHost()
        {
            _testHostFactory = new TestHostFactory(ConfigureServices, Configure);
            Server = _testHostFactory.Server;
        }

        protected sealed override IServiceProvider ServiceProviderFactory() => Server.Services;

        protected virtual void Configure(IApplicationBuilder app)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            _testHostFactory?.Dispose();
        }
    }
}