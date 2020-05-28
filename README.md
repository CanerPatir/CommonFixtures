CommonFixtures
========

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/CanerPatir/Commonfixtures/main.yml)
[![codecov](https://codecov.io/gh/CanerPatir/CommonFixtures/branch/master/graph/badge.svg?token=OSKYBSA9KW)](https://img.shields.io/codecov/c/github/CanerPatir/CommonFixtures?token=OSKYBSA9KW)
![Nuget](https://img.shields.io/nuget/v/CommonFixtures?color=blue)

Common fixture is a toolkit contains essential test fixtures for .net core and asp.net core test projects. CommonFixtures aims 
to reduce your preparing test suite effort for common testing concerns and supplying ready to use testing infrastructure.
CommonFixtures can be used independently from testing framework. You can prefer any kind of dotNet testing framework like xUnit, nUnit etc. .

Ready to use fixtures
========
CommonFixtures supplies some base classes to supply underlying fixtures for your test suite. These are as below;
    
* [BaseTest](#basetest)
* [WithIoC](#withioc)
* [WithHost](#withhost)
* [WithWebApp](#withwebapp)
* [WithEfCore](#withefcore)
* [WithWebAppAndEfCore](#withwebappandefcore)

## BaseTest 

Contains basic mocking and stubbing methods to supply standard DSL for all tests. (It uses FakeItEasy library for faking and AutoFixture for data generation internally)

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

ServiceCollection fixture to keep your IoC logic afloat throughout the test. 

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

Supplies test host for testing .net core hosted services (see: [.NET Core Hosted Services](https://docs.microsoft.com/tr-tr/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio))

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
        // Act
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

## WithWebApp 

Aspnet core test server fixture for integration testing scenarios

```csharp
public class MvcIntegrationTest : WithWebApp<Startup>
{
    protected override void ConfigureServices(IServiceCollection services)
    {        
        // You can override or mock services that registered from Startup.cs before
    }

    [Fact]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = HttpClient;

        // Act
        var response = await client.GetAsync("/weatherforecast");

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
    }
}
```
> :warning: Ensure _Microsoft.AspNetCore.Mvc.Testing_ package added to root of your test project before using this fixture.

## WithEfCore 

* Supplies test fixture to test projects which use Entity Framework Core as persistence layer. 
* CommonFixtures automatically replaces your existing ef core db context configuration with in memory sqlLite. 
* Provides useful approach when you want to test your persistence logic like db model, entity validations, secend level cache, repository abstractions without mocking anything.

```csharp
public class ProductRepositoryTest : WithEfCore<ApplicationDbContext>
{
    [Fact]
    public async Task Should_Persist_New_Created_Product()
    {
        // Arrange
        Arrange(dbContext =>
        {
            // helper that pull the database to desired state before acting (optional)
        });

        var sut = new ProductRepository(DbContext);

        // Act

        var id = await sut.CreateProduct(Random<Product>(), CancellationToken.None);

        // Assert
        NewServiceScope(); // dispose all of services and recreate new one to simulate new scope like new http request
        Assert.IsType<int>(id);
        Assert.NotEqual(default, id);
        var createdProduct = Get<Product>(id);
        Assert.NotNull(createdProduct);
    }
}
```

## WithWebAppAndEfCore 

* Combination of WithWebApp and WithEfCore
* In addition to the _WithEfCore_, starts test server up and running then replace db context which registered in the Startup.ConfigureServices method
* It is useful when you want to make test more complex application logic without mocking persistence layer

```csharp
// Automatically replaces your db context configuration specified in Startup.cs with in memory sqlLite
public class CreateProductCommandHandlerTest : WithWebAppAndEfCore<Startup, ApplicationDbContext>
{
    [Fact]
    public async Task Should_Created_New_Product()
    {
        // Arrange
        Arrange(dbContext =>
        {
            // you can use pull the database to desired state before acting
            // dbContext.Categories.Add(Random<Category>());
        });
        
        var mediator = GetService<IMediator>();

        // Act
        var id = await mediator.Send(Random<CreateProductCommand>());

        // Assert
        NewServiceScope(); // dispose all of services and recreate new one to simulate new scope like new http request
        Assert.IsType<int>(id);
        Assert.NotNull(Get<Product>(id));
    }
}
```

### Supported .Net Core Versions

|              |
|--------------|
|.netocreapp3.1|
|.netocreapp3.0|



