using System;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    public abstract class WithIoC : BaseTest
    {
        private readonly Lazy<IServiceProvider> _rootServiceProvider;
        private IServiceScope _serviceScope;

        protected WithIoC()
        {
            _rootServiceProvider = new Lazy<IServiceProvider>(ServiceProviderFactory);
        }
        
        protected virtual IServiceProvider ServiceProviderFactory()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Creates new service scope. It can be useful to simulate new request or isolate assert phase from arrange
        /// </summary>
        /// <returns>New service scope</returns>
        protected IServiceScope NewServiceScope()
        {
            _serviceScope?.Dispose();
            return _serviceScope = _rootServiceProvider?.Value?.CreateScope();
        }

        /// <summary>
        /// Gets service from current service scope. It creates new scope if no scope exists
        /// </summary>
        /// <typeparam name="T">Type of IoC service</typeparam>
        /// <returns>IoC service</returns>
        protected T GetService<T>()
        {
            if (_serviceScope == null)
            {
                NewServiceScope();
            }

            return _serviceScope.ServiceProvider.GetRequiredService<T>();
        }

        public override void Dispose()
        {
            base.Dispose();
            _serviceScope?.Dispose();
        }
    }
}