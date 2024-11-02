using MassTransit;
using Resourcerer.Dtos.Fake;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Senders.Fake;

public class FakeCommandSender : IMessageSender<FakeCommandDto>
{
    private readonly IBus _bus;

    public FakeCommandSender(IBus bus)
    {
        _bus = bus;
    }
    public Task SendAsync(FakeCommandDto message)
    {
        Console.WriteLine("Sending command");
        return _bus.Send(message);
    }
}
