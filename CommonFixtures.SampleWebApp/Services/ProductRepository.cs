using System.Threading;
using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Model;

namespace CommonFixtures.SampleWebApp.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateProduct(Product product, CancellationToken cancellationToken)
        {
            await _dbContext.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        public Task<Product> GetProduct(int id, CancellationToken cancellationToken)
        {
            return _dbContext.FindAsync<Product>(id).AsTask();
        }

        public async Task SaveProduct(Product product, CancellationToken cancellationToken)
        {
            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}