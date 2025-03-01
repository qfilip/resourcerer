using MassTransit;

namespace Resourcerer.Messaging.MassTransit;

public class ConsumerBase<TMessage> : IConsumer<TMessage> where TMessage : class
{
    private readonly Func<TMessage, Task> _handleMethod;

    public ConsumerBase(Func<TMessage, Task> handleMethod)
    {
        _handleMethod = handleMethod;
    }

    public virtual async Task Consume(ConsumeContext<TMessage> context)
    {
        await _handleMethod(context.Message);
    }
}
