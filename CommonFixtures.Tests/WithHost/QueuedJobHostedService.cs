using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CommonFixtures.Tests.WithHost
{
    public class QueueManager
    {
        private readonly ChannelWriter<Action> _writer;

        public QueueManager(ChannelWriter<Action> writer)
        {
            _writer = writer;
        }

        public async void EnqueueJob(Action action) => await _writer.WriteAsync(action);
    }

    public class QueuedJobHostedService : BackgroundService
    {
        private readonly ChannelReader<Action> _reader;

        public QueuedJobHostedService(ChannelReader<Action> reader)
        {
            _reader = reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _reader.WaitToReadAsync(stoppingToken))
            {
                while (_reader.TryRead(out var workItem))
                {
                    workItem();
                }
            }
        }
    }
}