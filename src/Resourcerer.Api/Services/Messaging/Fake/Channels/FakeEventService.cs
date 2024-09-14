using Resourcerer.Application.Messaging.Abstractions;
using Resourcerer.Application.Messaging.Channels;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Fake;
using Resourcerer.Logic.Fake;

namespace Resourcerer.Api.Services.Messaging.Fake.Channels;

public class FakeEventService : ChannelConsumerHostingService<FakeCommandDto>
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
