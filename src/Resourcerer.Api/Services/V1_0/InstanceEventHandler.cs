﻿using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Events;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class InstanceEventHandler : BackgroundService
{
    private readonly ChannelReader<EventDtoBase> _reader;
    private readonly IServiceProvider _serviceProvider;

    public InstanceEventHandler(
        ChannelReader<EventDtoBase> reader,
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
                await HandleEvent(message, database);
            }
        }
    }

    private static Task HandleEvent(EventDtoBase message, AppDbContext appDbContext)
    {
        if (message is InstanceOrderRequestDto orderEv)
        {
            var handler = new CreateInstanceOrderedEvent.Handler(appDbContext);
            return handler.Handle(orderEv);
        }
        else if (message is InstanceCancelRequestDto cancelEv)
        {
            var handler = new CreateInstanceOrderCancelledEvent.Handler(appDbContext);
            return handler.Handle(cancelEv);
        }
        else if (message is InstanceDeliveredRequestDto deliverEv)
        {
            var handler = new CreateInstanceDeliveredEvent.Handler(appDbContext);
            return handler.Handle(deliverEv);
        }
        else if (message is InstanceDiscardedRequestDto discardEv)
        {
            var handler = new CreateInstanceDiscardedEvent.Handler(appDbContext);
            return handler.Handle(discardEv);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }
}