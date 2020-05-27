using System;

namespace CommonFixtures
{
    public abstract class WithAppServer<TStartup> : WithIoC
        where TStartup : class
    {
        protected TestWebAppFactory<TStartup> WebAppFactory { get; }

        protected WithAppServer()
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