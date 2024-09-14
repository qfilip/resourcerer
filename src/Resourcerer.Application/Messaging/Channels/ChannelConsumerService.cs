using Resourcerer.Application.Messaging.Abstractions;
using System.Threading.Channels;

namespace Resourcerer.Application.Messaging.Channels;

public class ChannelConsumerService<TMessage> : IMessageReader<TMessage>
{
    private readonly ChannelReader<TMessage> _reader;
    public ChannelConsumerService(ChannelReader<TMessage> reader)
    {
        _reader = reader;
    }

    public bool IsCompleted() => _reader.Completion.IsCompleted;

    public Task<TMessage> ReadAsync() => _reader.ReadAsync().AsTask();
}
