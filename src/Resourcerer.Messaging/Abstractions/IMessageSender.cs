namespace Resourcerer.Messaging.Abstractions;

public interface IMessageSender<TMessage>
{
    Task SendAsync(TMessage message);
}
