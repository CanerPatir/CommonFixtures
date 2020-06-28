using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    /// <summary>
    /// port awareness test server
    /// </summary>
    public class TestWebAppFactory<TStartup> : WebApplicationFactory<TStartup>, IWebAppFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureServices;

        private readonly Lazy<HttpClient> _httpClient;
        public HttpClient HttpClient => _httpClient.Value;


        private readonly Lazy<Uri> _rootUri;
        public Uri RootUri => _rootUri.Value;

        internal TestWebAppFactory(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices ?? throw new ArgumentNullException(nameof(configureServices));
            _httpClient = new Lazy<HttpClient>(CreateClient);
            _rootUri = new Lazy<Uri>(() => Server.BaseAddress); //Last is https://localhost:5001!
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                _configureServices(services);

                var transientSp = services.BuildServiceProvider();

                using var scope = transientSp.CreateScope();
                InvokeMigrateMethodOfStartup(scope.ServiceProvider);
            });
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