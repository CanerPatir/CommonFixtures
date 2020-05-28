using System.Threading;
using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Model;

namespace CommonFixtures.SampleWebApp.Services
{
    public interface IProductRepository
    {
        Task<int> CreateProduct(Product product, CancellationToken cancellationToken);
        Task<Product> GetProduct(int id, CancellationToken cancellationToken);
        Task SaveProduct(Product product, CancellationToken cancellationToken);
    }
}