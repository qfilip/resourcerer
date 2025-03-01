using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.V1.Instances;

public class InstanceDiscardEventService : ChannelConsumerHostingService<V1InstanceDiscardCommand>
{
    public InstanceDiscardEventService(
        IMessageReader<V1InstanceDiscardCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1InstanceDiscardCommand message, AppDbContext appDbContext)
    {
        var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
        return handler.Handle(message);
    }
}
