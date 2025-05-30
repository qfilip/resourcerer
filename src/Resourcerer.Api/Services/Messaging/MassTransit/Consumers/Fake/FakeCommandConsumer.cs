﻿using MassTransit;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.Fake;

public class FakeCommandConsumer : ConsumerBase<FakeCommandDto>
{
    public FakeCommandConsumer(FakeCommandEventHandler.Handler handler)
        : base(handler.Handle) { }

    public override async Task Consume(ConsumeContext<FakeCommandDto> context)
    {
        Console.WriteLine("Consuming command");
        await base.Consume(context);
    }
}
