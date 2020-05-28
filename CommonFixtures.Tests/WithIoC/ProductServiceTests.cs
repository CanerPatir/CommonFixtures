using System.Threading;
using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Model;
using CommonFixtures.SampleWebApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CommonFixtures.Tests.WithIoC
{
    public class ProductServiceTests : CommonFixtures.WithIoC
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.MockAndRegister<IProductRepository>();
            services.Register<ProductService>();
        }
        
        [Fact]
        public async Task Should_Created_Product()
        {
            // Given
            var givenTitle = Random<string>();
            var givenPrice = Random<decimal>();
            var mockRepository = GetService<IProductRepository>();
            var sut = GetService<ProductService>();
            
            // When
            await sut.CreateProduct(givenTitle, givenPrice);

            // Then
            Verify(() => mockRepository.CreateProduct(ArgMatches<Product>(x => x.Title == givenTitle && x.Price == givenPrice), ArgIgnore<CancellationToken>()), numberOfTimes: 1);
        }
    }
}