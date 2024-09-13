namespace Resourcerer.Application.Messaging.Abstractions;

public interface IMessageSender<TMessage>
{
    Task SendAsync(TMessage message);
}
