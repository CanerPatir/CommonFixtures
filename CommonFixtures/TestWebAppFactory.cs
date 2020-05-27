using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    public class TestWebAppFactory<TStartup> : WebApplicationFactory<TStartup> 
        where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureServices;

        internal TestWebAppFactory(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices ?? throw new ArgumentNullException(nameof(configureServices));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(_configureServices);
            builder.Configure(app =>
            {
                // invoking migrate method of startup
                InvokeMigrateMethodOfStartup(app);
            });
        }
        
        private static void InvokeMigrateMethodOfStartup(IApplicationBuilder app)
        {
            var methodInfo = typeof(TStartup).GetMethod("Migrate", BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null) return;

            var result = methodInfo.Invoke(null, new object[]
            {
                app.ApplicationServices, CancellationToken.None
            });

            if (result is Task taskResult)
            {
                taskResult.ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}