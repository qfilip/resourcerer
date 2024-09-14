using MassTransit;
using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Dtos.Fake;

namespace Resourcerer.Api.Services.Messaging.Fake.MassTransit.Senders;

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
