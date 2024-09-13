using MassTransit;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services.Messaging.V1.MassTransit.Senders;

public class InstanceDiscardCommandSender : IMessageSender<V1InstanceDiscardCommand>
{
    private readonly IBus _bus;

    public InstanceDiscardCommandSender(IBus bus)
    {
        _bus = bus;
    }

    public Task SendAsync(V1InstanceDiscardCommand message) => _bus.Send(message);
}
