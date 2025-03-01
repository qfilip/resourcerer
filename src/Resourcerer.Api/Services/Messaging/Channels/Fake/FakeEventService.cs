using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.Fake;

public class FakeEventService : ChannelConsumerHostingService<FakeCommandDto>
{
    public FakeEventService(
        IMessageReader<FakeCommandDto> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }
    protected override Task HandleEvent(FakeCommandDto message, AppDbContext appDbContext)
    {
        var handler = new FakeCommandEventHandler.Handler(appDbContext);
        return handler.Handle(message);
    }
}
