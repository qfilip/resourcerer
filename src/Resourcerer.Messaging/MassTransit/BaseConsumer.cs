using MassTransit;

namespace Resourcerer.Messaging.MassTransit;

public class BaseConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
{
    private readonly Func<TMessage, Task> _handleMethod;

    public BaseConsumer(Func<TMessage, Task> handleMethod)
    {
        _handleMethod = handleMethod;
    }

    public virtual async Task Consume(ConsumeContext<TMessage> context)
    {
        await _handleMethod(context.Message);
    }
}
