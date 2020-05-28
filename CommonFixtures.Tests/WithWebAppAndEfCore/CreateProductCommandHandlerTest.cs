using System.Threading.Tasks;
using CommonFixtures.SampleWebApp;
using CommonFixtures.SampleWebApp.Controllers;
using CommonFixtures.SampleWebApp.Model;
using MediatR;
using Xunit;

namespace CommonFixtures.Tests.WithWebAppAndEfCore
{
    /// <summary>
    /// Automatically replaces your db context configuration specified in Startup.cs with in memory sqlLite
    /// </summary>
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
}