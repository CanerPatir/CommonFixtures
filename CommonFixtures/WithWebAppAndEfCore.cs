using System;
using Microsoft.EntityFrameworkCore;

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
        protected TestWebAppFactory<TStartup> WebAppFactory { get; }

        protected WithWebAppAndEfCore()
        {
            WebAppFactory = new TestWebAppFactory<TStartup>(services =>
                {
                    ConfigureServices(services);
                    ReplaceDbContext(services);
                }
            );
        }

        protected override IServiceProvider ServiceProviderFactory() => WebAppFactory.Services;

        public override void Dispose()
        {
            base.Dispose();
            WebAppFactory?.Dispose();
        }
    }
}