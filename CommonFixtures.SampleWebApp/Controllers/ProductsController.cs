using System.Threading.Tasks;
using CommonFixtures.SampleWebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CommonFixtures.SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public Task<int> Create(CreateProductCommand command)
        {
            return _productService.CreateProduct(command.Title, command.Price);
        }
    }

    public class CreateProductCommand : IRequest<int>
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}