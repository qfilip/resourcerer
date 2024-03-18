using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Logic.V1_0.Commands;

namespace Resourcerer.Api.Services.V1_0;

public class InstanceDiscardEventService : EventConsumerServiceBase<InstanceDiscardedRequestDto>
{
    public InstanceDiscardEventService(
        IConsumerAdapter<InstanceDiscardedRequestDto> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) {}

    protected override Task HandleEvent(InstanceDiscardedRequestDto message, AppDbContext appDbContext)
    {
        var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
        return handler.Handle(message);
    }
}
