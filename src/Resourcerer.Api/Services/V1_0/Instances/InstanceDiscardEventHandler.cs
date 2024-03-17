using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class InstanceDiscardEventHandler : EventServiceBase<InstanceDiscardedRequestDto>
{
    public InstanceDiscardEventHandler(ChannelReader<InstanceDiscardedRequestDto> reader, IServiceProvider serviceProvider) : base(reader, serviceProvider)
    {
    }

    protected override Task HandleEvent(InstanceDiscardedRequestDto message, AppDbContext appDbContext)
    {
        var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
        return handler.Handle(message);
    }
}
