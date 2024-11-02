using Resourcerer.Messaging.Abstractions;
using System.Threading.Channels;

namespace Resourcerer.Messaging.Channels;

public class ChannelSenderService<TMessage> : IMessageSender<TMessage>
{
    private readonly ChannelWriter<TMessage> _channel;

    public ChannelSenderService(ChannelWriter<TMessage> channel)
    {
        _channel = channel;
    }
    public Task SendAsync(TMessage message) =>
        _channel.WriteAsync(message).AsTask();
}
