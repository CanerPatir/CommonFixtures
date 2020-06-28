using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium.Chrome;

namespace CommonFixtures
{
    /// <summary>
    /// port aware test server
    /// </summary>
    public class SeleniumTestWebAppFactory<TStartup> : WebApplicationFactory<TStartup>, IWebAppFactory<TStartup>
        where TStartup : class
    {
        private readonly CancellationTokenSource _seleniumCancellation;
        private readonly Action<IServiceCollection> _configureServices;
        private readonly int _port;
        private readonly string _localhostBaseAddress;
        private IWebHost _host;

        private readonly Lazy<HttpClient> _httpClient;
        public HttpClient HttpClient => _httpClient.Value;
        public ChromeDriver Selenium { get; }
        public Uri RootUri { get; private set; }

        internal SeleniumTestWebAppFactory(Action<IServiceCollection> configureServices, bool seleniumHeadless)
        {
            _seleniumCancellation = new CancellationTokenSource();
            _configureServices = configureServices;
            _port = GetRandomUnusedPort();
            _localhostBaseAddress = "https://localhost:" + _port;

            ClientOptions.BaseAddress = new Uri(_localhostBaseAddress);

            CreateServer(
                CreateWebHostBuilder()
            );
            _httpClient = new Lazy<HttpClient>(CreateClient);
            Selenium = new Selenium(RootUri, seleniumHeadless).CreateBrowser(_seleniumCancellation.Token);
        }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint) listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            _host = builder
                .PreferHostingUrls(true)
                .UseUrls(_localhostBaseAddress)
                .Build();
            _host.Start();
            RootUri = new Uri(_host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault());
            // not used but needed in the CreateServer method logic
            return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseStartup<TStartup>();
            builder.ConfigureServices(services =>
            {
                _configureServices(services);

                var transientSp = services.BuildServiceProvider();

                using var scope = transientSp.CreateScope();
                InvokeMigrateMethodOfStartup(scope.ServiceProvider);
            });
            return builder;
        }

        [ExcludeFromCodeCoverage]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _host?.Dispose();
                _seleniumCancellation?.Cancel();
                Selenium?.Dispose();
            }
        }

        private static void InvokeMigrateMethodOfStartup(IServiceProvider serviceProvider)
        {
            var methodInfo = typeof(TStartup).GetMethod("Migrate", BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null) return;

            var result = methodInfo.Invoke(null, new object[]
            {
                serviceProvider, CancellationToken.None
            });

            if (result is Task taskResult)
            {
                taskResult.ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}