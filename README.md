CommonFixtures
========
A toolkit contains essential test fixtures for .net core and asp.net core projects. CommonFixtures aims 
to reduce your preparing test suit effort for common testing concerns and supplying ready to use test infrastructure.
CommonFixture can be used independent from testing tool library. You can prefer any kind of dotNet testing framework like xUnit, nUnit etc. .

Ready to use fixtures
========
CommonFixtures supplies some base classes to supply underlying fixtures for your test suit. These are as below;
    
    * BaseTest
    * WithIoC
    * WithHost
    * WithAppServer
    * WithEfCore
    * WithWebAppAndEfCore

## BaseTest 

* Contains basic mocking and stubbing methods to supply standard DSL for all tests. (It uses FakeItEasy library for faking and AutoFixture for data generation internally)

* Stubbing demo
```csharp
    public class ProductServiceTests : BaseTest
    {
        private readonly ProductService _sut;
        private readonly IProductRepository _mockRepository;

        public ProductServiceTests()
        {
            _mockRepository = Mock<IProductRepository>();
            _sut = new ProductService(_mockRepository);
        }

        // with your favourite testing tool xUnit, nunit ...  etc.
        // [Fact]
        // [Test]
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
    }
```
* Verification demo
```csharp
        public async Task Should_Call_Repo_Once()
        {
            // Given
            var givenTitle = Random<string>();
            var givenPrice = Random<decimal>();
            
            // When
            await _sut.CreateProduct(givenTitle, givenPrice);

            // Then
            Verify(() => _mockRepository.CreateProduct(ArgMatches<Product>(x => x.Title == givenTitle && x.Price == givenPrice), ArgIgnore<CancellationToken>()), numberOfTimes: 1);
        }
```

## WithIoC 

* ServiceCollection fixture to keep your IoC logic afloat throughout the test. 

```csharp
    public class ProductServiceTest : WithIoC
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
```

* Also allows you to mock some dependencies of the system under test while leaving others as they are
```csharp
  public class DependencyMockingTest : WithIoC
  {
        public class YourAwesomeService
        {
             public YourAwesomeService(IDependency1 dep1, IDependency2 dep2)
             {
                _dep1 = dep1;
                _dep2 = dep2;
             }

             // ....
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.MockAndRegister<IDependency1>();
            services.Register<YourAwesomeService>();
            // so mocked just dependency 1 and leaving second one as it is
        }

        // ....
   }
```

## WithHost

* Supplies test host for testing .net core hosted services (see: [.NET Core Hosted Services](https://docs.microsoft.com/tr-tr/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio))

```csharp
    public class QueuedJobHostedServiceTests : WithHost
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            // adding your awesome hosted service
            services.AddHostedService<QueuedJobHostedService>();
        }
        
        [Fact]
        public async Task It_Should_Execute_Queued_Task()
        {
            // Arrange
            var queueManager = GetService<QueueManager>();

            var counter = 0;

            queueManager.EnqueueJob(() =>
            {
                Interlocked.Increment(ref counter);
            });
            
            // Assert
            await Task.Delay(300);
            Assert.Equal(1, counter);
        }
    }
```

## WithAppServer 

* Aspnet core test server fixture for integration testing scenarios

```csharp
```

## WithEfCore 

* Supplies test fixture to test projects which use Entity Framework Core as persistence layer.

```csharp
```

## WithWebAppAndEfCore 

* Entity framework test fixture 

```csharp
```



