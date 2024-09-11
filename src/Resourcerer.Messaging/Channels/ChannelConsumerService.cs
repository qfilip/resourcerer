using Resourcerer.Messaging.Abstractions;
using System.Diagnostics;
using System.Threading.Channels;

namespace Resourcerer.Messaging.Channels;

public class ChannelConsumerService<TMessage> : IMessageConsumer<TMessage>
{
    private readonly ChannelReader<TMessage> _reader;
    public ChannelConsumerService(ChannelReader<TMessage> reader)
    {
        _reader = reader;
    }

    public bool IsCompleted() => _reader.Completion.IsCompleted;

    public Task<TMessage> ConsumeAsync() => _reader.ReadAsync().AsTask();
}
