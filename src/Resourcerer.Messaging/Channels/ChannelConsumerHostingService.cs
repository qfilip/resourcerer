using Microsoft.Extensions.Hosting;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Messaging.Channels;


public abstract class ChannelConsumerHostingService<TMessage, TRepository> : BackgroundService
{
    protected readonly IMessageReader<TMessage> _consumer;
    protected readonly IServiceProvider _serviceProvider;
    public ChannelConsumerHostingService(IMessageReader<TMessage> consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }

    protected abstract Task HandleEvent(TMessage message, TRepository repository);
}

