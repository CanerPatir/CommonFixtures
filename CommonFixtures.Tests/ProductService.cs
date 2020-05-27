using System.Threading;
using System.Threading.Tasks;

namespace CommonFixtures.Tests
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        
        public Task<int> CreateProduct(string title, decimal price)
        {
            var product = new Product
            {
                Price =  price,
                Title = title
            };
            
            return _repository.CreateProduct(product, CancellationToken.None);
        }

        public async Task UpdateProductPrice(int id, decimal givenPrice)
        {
            var product=  await _repository.GetProduct(id, CancellationToken.None);

            product.Price = givenPrice;

            await _repository.SaveProduct(product, CancellationToken.None);
        }
    }
}