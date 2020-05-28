using System;
using System.Net.Http;

namespace CommonFixtures
{
    public abstract class WithWebApp<TStartup> : WithIoC
        where TStartup : class
    {
        protected TestWebAppFactory<TStartup> WebAppFactory { get; }
        protected HttpClient HttpClient => WebAppFactory.HttpClient;

        protected WithWebApp()
        {
            WebAppFactory = new TestWebAppFactory<TStartup>(ConfigureServices);
        }
        
        protected sealed override IServiceProvider ServiceProviderFactory() => WebAppFactory.Services;

        public override void Dispose()
        {
            base.Dispose();
            WebAppFactory?.Dispose();
        }
    }
}