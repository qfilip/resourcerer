﻿using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;

namespace Resourcerer.Api.Services.V1;

public class InstanceDiscardEventService : ChannelConsumerHostingService<V1InstanceDiscardedRequest>
{
    public InstanceDiscardEventService(
        IMessageConsumer<V1InstanceDiscardedRequest> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) {}

    protected override Task HandleEvent(V1InstanceDiscardedRequest message, AppDbContext appDbContext)
    {
        var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
        return handler.Handle(message);
    }
}
