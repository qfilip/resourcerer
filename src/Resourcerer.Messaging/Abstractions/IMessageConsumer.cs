namespace Resourcerer.Messaging.Abstractions;

public interface IMessageConsumer<TMessage>
{
    bool IsCompleted();
    Task<TMessage> ConsumeAsync();
}
