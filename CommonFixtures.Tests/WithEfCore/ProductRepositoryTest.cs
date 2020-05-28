using System.Threading;
using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Model;
using CommonFixtures.SampleWebApp.Services;
using Xunit;

namespace CommonFixtures.Tests.WithEfCore
{
    public class ProductRepositoryTest : WithEfCore<ApplicationDbContext>
    {
        [Fact]
        public async Task Should_Persist_New_Created_Product()
        {
            // Arrange
            Arrange(dbContext =>
            {
                // Arrange is a helper that pull the database to desired state before acting (optional)
            });

            var productRepo = new ProductRepository(DbContext);
            // Act

            var id = await productRepo.CreateProduct(Random<Product>(), CancellationToken.None);

            // Assert
            NewServiceScope(); // dispose all of services and recreate new one to simulate new scope like new http request
            Assert.IsType<int>(id);
            Assert.NotEqual(default, id);
            var createdProduct = Get<Product>(id);
            Assert.NotNull(createdProduct);
        }
    }
}