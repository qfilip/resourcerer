using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services.Messaging.Channels.V1.Examples;

public class ExampleEventService : ChannelConsumerHostingService<V1ExampleCommand>
{
    public ExampleEventService(
        IMessageReader<V1ExampleCommand> consumer,
        IServiceProvider serviceProvider) : base(consumer, serviceProvider) { }

    protected override Task HandleEvent(V1ExampleCommand message, AppDbContext appDbContext)
    {
        Func<Task<HandlerResult<Unit>>> handler = message switch
        {
            V1CreateExampleCommand create =>
                () => new CreateExample.Handler(appDbContext).Handle(create),

            V1UpdateExampleCommand update =>
                () => new UpdateExample.Handler(appDbContext).Handle(update),

            _ => throw new InvalidOperationException("Unsupported event type")
        };

        return handler();
    }
}
