using System.Threading;
using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Controllers;
using CommonFixtures.SampleWebApp.Model;
using MediatR;

namespace CommonFixtures.SampleWebApp.Services
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<int> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Price = command.Price,
                Title = command.Title
            };

            return _productRepository.CreateProduct(product, CancellationToken.None);
        }
    }
}