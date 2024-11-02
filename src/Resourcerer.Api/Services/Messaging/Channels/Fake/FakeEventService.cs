using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.Fake;

public class FakeEventService : ChannelConsumerHostingServiceBase<FakeCommandDto>
{
    public FakeEventService(
        IMessageReader<FakeCommandDto> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }
    protected override async Task HandleEvent(FakeCommandDto message, AppDbContext appDbContext)
    {
        var handler = new FakeCommandEventHandler.Handler(appDbContext);
        await handler.Handle(message);
    }
}
