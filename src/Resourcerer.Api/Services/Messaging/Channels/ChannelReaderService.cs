using System.Threading.Channels;

namespace Resourcerer.Api.Services.Messaging.Channels;

public class ChannelReaderService<TMessage> : IConsumerAdapter<TMessage>
{
    private readonly ChannelReader<TMessage> _reader;

    public ChannelReaderService(ChannelReader<TMessage> reader)
    {
        _reader = reader;
    }

    public bool IsCompleted() => _reader.Completion.IsCompleted;

    public Task<TMessage> ReadAsync() => _reader.ReadAsync().AsTask();
}
