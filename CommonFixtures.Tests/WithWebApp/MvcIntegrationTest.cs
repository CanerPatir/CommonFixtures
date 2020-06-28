using System.Threading.Tasks;
using CommonFixtures.SampleWebApp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CommonFixtures.Tests.WithWebApp
{
    public class MvcIntegrationTest : WithWebApp<Startup>
    {
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