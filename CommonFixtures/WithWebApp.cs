using System;
using System.Net.Http;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace CommonFixtures
{
    public abstract class WithWebApp<TStartup> : WithIoC
        where TStartup : class
    {
        protected IWebAppFactory<TStartup> WebAppFactory { get; }
        protected HttpClient HttpClient => WebAppFactory.HttpClient;
        protected ChromeDriver Selenium => (WebAppFactory as SeleniumTestWebAppFactory<TStartup>)?.Selenium;
        protected virtual bool SeleniumEnabled => false;
        protected virtual bool SeleniumHeadless => true;
        protected virtual bool SeleniumRandomPort => false;

        protected WithWebApp()
        {
            if (SeleniumEnabled)
            {
                WebAppFactory = new SeleniumTestWebAppFactory<TStartup>(ConfigureServices, SeleniumHeadless, SeleniumRandomPort);
            }
            else
            {
                WebAppFactory = new TestWebAppFactory<TStartup>(ConfigureServices);
            }
        }

        protected sealed override IServiceProvider ServiceProviderFactory() => WebAppFactory.Services;

        public override void Dispose()
        {
            base.Dispose();
            WebAppFactory?.Dispose();
     
        }
    }
}