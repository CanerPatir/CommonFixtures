namespace CommonFixtures.Tests.BaseTest
{
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using CommonFixtures;
    
    public class ProductServiceTests : BaseTest
    {
        private readonly ProductService _sut;
        private readonly IProductRepository _mockRepository;

        public ProductServiceTests()
        {
            _mockRepository = Mock<IProductRepository>();
            _sut = new ProductService(_mockRepository);
        }

        [Fact]
        public async Task Should_Created_Product()
        {
            // Given
            var givenTitle = Random<string>();
            var givenPrice = Random<decimal>();
            var expectedId = Random<int>();
            
            StubAsync(() => _mockRepository.CreateProduct(ArgMatches<Product>(x => x.Title == givenTitle && x.Price == givenPrice), ArgIgnore<CancellationToken>()), expectedId);

            // When
            var id =  await _sut.CreateProduct(givenTitle, givenPrice);

            // Then
            Assert.Equal(expectedId, id);
        }
        
        [Fact]
        public async Task Should_Call_Repo()
        {
            // Given
            var givenTitle = Random<string>();
            var givenPrice = Random<decimal>();
            
            // When
            await _sut.CreateProduct(givenTitle, givenPrice);

            // Then
            Verify(() => _mockRepository.CreateProduct(ArgMatches<Product>(x => x.Title == givenTitle && x.Price == givenPrice), ArgIgnore<CancellationToken>()), numberOfTimes: 1);
        }
    }
}