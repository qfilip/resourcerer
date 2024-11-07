using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;

namespace Resourcerer.Api.Services.Messaging.Channels;

public abstract class ChannelConsumerHostingServiceBase<TMessage> : ChannelConsumerHostingService<TMessage, AppDbContext>
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
            using var scope = _serviceProvider.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            if (dbContext == null)
            {
                throw new InvalidOperationException("Failed to resolve database service");
            }

            // TODO: map to signalR hub
            await HandleEvent(message, dbContext);
            
        }
    }
}
