using MassTransit;
using Resourcerer.Api.Services.Messaging.V1.MassTransit.Consumers;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Services.Messaging.Fake.MassTransit.Consumers;

public class FakeCommandConsumer : BaseConsumer<FakeCommandDto>
{
    public FakeCommandConsumer(FakeCommandEventHandler.Handler handler)
        : base(handler.Handle) { }

    public override async Task Consume(ConsumeContext<FakeCommandDto> context)
    {
        Console.WriteLine("Consuming command");
        await base.Consume(context);
    }
}
