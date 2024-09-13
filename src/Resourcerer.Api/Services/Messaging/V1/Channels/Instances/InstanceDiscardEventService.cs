using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Channels;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;

namespace Resourcerer.Api.Services.Messaging.V1.Channels.Instances;

public class InstanceDiscardEventService : ChannelConsumerHostingService<V1InstanceDiscardedRequest>
{
    public InstanceDiscardEventService(
        IMessageConsumer<V1InstanceDiscardedRequest> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1InstanceDiscardedRequest message, AppDbContext appDbContext)
    {
        var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
        return handler.Handle(message);
    }
}
