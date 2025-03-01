using Resourcerer.Messaging.Abstractions;
using System.Threading.Channels;

namespace Resourcerer.Messaging.Channels;

public class ChannelSenderServiceBase<TMessage> : IMessageSender<TMessage>
{
    private readonly ChannelWriter<TMessage> _channel;

    public ChannelSenderServiceBase(ChannelWriter<TMessage> channel)
    {
        _channel = channel;
    }
    public Task SendAsync(TMessage message) =>
        _channel.WriteAsync(message).AsTask();
}
