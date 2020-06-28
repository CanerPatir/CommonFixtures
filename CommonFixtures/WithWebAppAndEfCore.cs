using System;
using System.Net.Http;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Chrome;

namespace CommonFixtures
{
    public abstract class WithWebAppAndEfCore<TStartup, TDbContext> : WithWebAppAndEfCore<TStartup, TDbContext, TDbContext>
        where TStartup : class
        where TDbContext : DbContext
    {
    }

    public abstract class WithWebAppAndEfCore<TStartup, TDbContext, TDbContextImplementation> : WithEfCore<TDbContext, TDbContextImplementation>
        where TStartup : class
        where TDbContext : DbContext
        where TDbContextImplementation : class, TDbContext
    {
        protected IWebAppFactory<TStartup> WebAppFactory { get; }
        protected HttpClient HttpClient => WebAppFactory.HttpClient;
        protected ChromeDriver Selenium => (WebAppFactory as SeleniumTestWebAppFactory<TStartup>)?.Selenium;
        protected virtual bool SeleniumEnabled => false;
        protected virtual bool SeleniumHeadless => true;

        protected WithWebAppAndEfCore()
        {
            if (SeleniumEnabled)
            {
                WebAppFactory = new SeleniumTestWebAppFactory<TStartup>(services =>
                {
                    ConfigureServices(services);
                    ReplaceDbContext(services);
                }, SeleniumHeadless);
            }
            else
            {
                WebAppFactory = new TestWebAppFactory<TStartup>(services =>
                    {
                        ConfigureServices(services);
                        ReplaceDbContext(services);
                    }
                );
            }
        }

        protected override IServiceProvider ServiceProviderFactory() => WebAppFactory.Services;

        public override void Dispose()
        {
            base.Dispose();
            WebAppFactory?.Dispose();
        }
    }
}