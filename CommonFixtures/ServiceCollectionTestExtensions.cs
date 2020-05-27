using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    public static class ServiceCollectionTestExtensions
    {
        /// <summary>
        /// Mocks given service before register
        /// </summary>
        /// <param name="services"></param>
        /// <param name="strict">Specify mock type as strict</param>
        /// <typeparam name="T">Type of service</typeparam>
        /// <returns>Mocked service</returns>
        public static T MockAndRegister<T>(this IServiceCollection services, bool strict = false)
            where T : class
        {
            var mockService = BaseTest.Mock<T>(strict);
            services.Register(mockService);
            return mockService;
        }

        /// <summary>
        /// Registers given service instance
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <returns>service</returns>
        public static T Register<T>(this IServiceCollection services, T service)
            where T : class
        {
            services.AddSingleton(c => service);

            return service;
        }

        /// <summary>
        /// Registers given type service
        /// </summary>
        /// <typeparam name="T">Type of service</typeparam>
        /// <returns>service</returns>
        public static void Register<T>(this IServiceCollection services)
            where T : class
        {
            services.AddSingleton<T>();
        }
    }
}