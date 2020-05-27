using System.Threading;
using System.Threading.Tasks;

namespace CommonFixtures.Tests
{
    
    public interface IProductRepository
    {
        Task<int> CreateProduct(Product product, CancellationToken cancellationToken);
        Task<Product> GetProduct(int id, CancellationToken cancellationToken);
        Task SaveProduct(Product product, CancellationToken cancellationToken);
    }
    
    public class ProductRepository : IProductRepository
    {
        public Task<int> CreateProduct(Product product, CancellationToken cancellationToken)
        {
            // ...
            return Task.FromResult(1);
        }

        public Task<Product> GetProduct(int id, CancellationToken cancellationToken)
        {
            // ...
            return Task.FromResult(new Product());
        }

        public Task SaveProduct(Product product, CancellationToken cancellationToken)
        {
            // ...
            return Task.CompletedTask;
        }
    }
}