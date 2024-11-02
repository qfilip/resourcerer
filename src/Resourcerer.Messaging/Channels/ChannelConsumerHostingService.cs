using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Messaging.Channels;


public abstract class ChannelConsumerHostingService<TMessage> : BackgroundService
{
    private readonly IMessageReader<TMessage> _consumer;
    private readonly IServiceProvider _serviceProvider;
    public ChannelConsumerHostingService(IMessageReader<TMessage> consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!_consumer.IsCompleted())
        {
            var message = await _consumer.ReadAsync();
            using (var scope = _serviceProvider.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (database == null)
                {
                    throw new InvalidOperationException("Failed to resolve database service");
                }

                // TODO: map to signalR hub
                await HandleEvent(message, database);
            }
        }
    }

    protected abstract Task HandleEvent(TMessage message, AppDbContext appDbContext);
}

