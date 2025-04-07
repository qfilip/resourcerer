using MassTransit;
using Resourcerer.Dtos.V1;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Senders.V1.Examples;

public class ExampleCommandSender : IMessageSender<V1ExampleCommand>
{
    private readonly IBus _bus;
    public ExampleCommandSender(IBus bus)
    {
        _bus = bus;
    }

    public Task SendAsync(V1ExampleCommand message)
    {
        var task = message switch
        {
            V1CreateExampleCommand create => _bus.Send(create),
            V1UpdateExampleCommand update => _bus.Send(update),
            _ => throw new InvalidOperationException("Unsupported event type")
        };

        return task;
    }
}