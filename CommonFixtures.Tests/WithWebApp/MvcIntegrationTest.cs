using System.Threading.Tasks;
using CommonFixtures.SampleWebApp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CommonFixtures.Tests.WithWebApp
{
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
}