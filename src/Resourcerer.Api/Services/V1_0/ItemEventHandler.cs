using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Events;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class ItemEventHandler : BackgroundService
{
    private readonly ChannelReader<ItemEventDtoBase> _reader;
    private readonly IServiceProvider _serviceProvider;

    public ItemEventHandler(
        ChannelReader<ItemEventDtoBase> reader,
        IServiceProvider serviceProvider)
    {
        _reader = reader;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!_reader.Completion.IsCompleted)
        {
            var message = await _reader.ReadAsync();
            using (var scope = _serviceProvider.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (database == null)
                {
                    throw new InvalidOperationException("Failed to resolve database service");
                }

                // TODO: map to signalR hub
                var _ = await HandleEvent(message, database);
            }
        }
    }

    private static Task<HandlerResult<Unit>> HandleEvent(ItemEventDtoBase message, AppDbContext appDbContext)
    {
        if (message is ItemOrderedEventDto orderEv)
        {
            var handler = new CreateItemOrderedEvent.Handler(appDbContext);
            return handler.Handle(orderEv);
        }
        else if (message is ItemCancelledEventDto cancelEv)
        {
            var handler = new CreateItemCancelledEvent.Handler(appDbContext);
            return handler.Handle(cancelEv);
        }
        else if (message is ItemDeliveredEventDto deliverEv)
        {
            var handler = new CreateItemDeliveredEvent.Handler(appDbContext);
            return handler.Handle(deliverEv);
        }
        else if (message is ItemDiscardedEventDto discardEv)
        {
            var handler = new CreateItemDiscardedEvent.Handler(appDbContext);
            return handler.Handle(discardEv);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}
