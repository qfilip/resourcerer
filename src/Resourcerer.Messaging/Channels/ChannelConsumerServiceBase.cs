using Resourcerer.Messaging.Abstractions;
using System.Threading.Channels;

namespace Resourcerer.Messaging.Channels;

public class ChannelConsumerServiceBase<TMessage> : IMessageReader<TMessage>
{
    private readonly ChannelReader<TMessage> _reader;
    public ChannelConsumerServiceBase(ChannelReader<TMessage> reader)
    {
        _reader = reader;
    }

    public bool IsCompleted() => _reader.Completion.IsCompleted;

    public Task<TMessage> ReadAsync() => _reader.ReadAsync().AsTask();
}
