namespace Resourcerer.Application.Messaging.Abstractions;

public interface IMessageReader<TMessage>
{
    bool IsCompleted();
    Task<TMessage> ReadAsync();
}
