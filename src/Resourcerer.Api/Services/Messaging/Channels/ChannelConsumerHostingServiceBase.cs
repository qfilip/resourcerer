using Resourcerer.DataAccess.Contexts;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;

namespace Resourcerer.Api.Services.Messaging.Channels;

public abstract class ChannelConsumerHostingServiceBase<TMessage> : ChannelConsumerHostingService<TMessage>
{
    public ChannelConsumerHostingServiceBase(
        IMessageReader<TMessage> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider)
    {}

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
}
