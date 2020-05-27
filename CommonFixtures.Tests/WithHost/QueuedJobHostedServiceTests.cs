using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CommonFixtures.Tests.WithHost
{
    public class QueuedJobHostedServiceTests : CommonFixtures.WithHost
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            var channel = Channel.CreateBounded<Action>(1);
            services.Register<QueueManager>(new QueueManager(channel.Writer));
            services.AddHostedService<QueuedJobHostedService>(sp => new QueuedJobHostedService(channel.Reader));
        }
        
        [Fact]
        public async Task It_should_execute_queued_task()
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
    
}