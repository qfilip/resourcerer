
using System.Threading.Channels;

namespace Resourcerer.Api.Services.Messaging;

public class ChannelSenderService<TMessage> : ISenderAdapter<TMessage>
{
    private readonly ChannelWriter<TMessage> _channel;

    public ChannelSenderService(ChannelWriter<TMessage> channel)
    {
        _channel = channel;
    }
    public Task SendAsync(TMessage message) =>
        _channel.WriteAsync(message).AsTask();
}
