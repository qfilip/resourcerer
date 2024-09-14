using MassTransit;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers;

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
