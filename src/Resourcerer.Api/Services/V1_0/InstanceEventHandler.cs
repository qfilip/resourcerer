using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Events;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class InstanceEventHandler : BackgroundService
{
    private readonly ChannelReader<InstanceEventDtoBase> _reader;
    private readonly IServiceProvider _serviceProvider;

    public InstanceEventHandler(
        ChannelReader<InstanceEventDtoBase> reader,
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

    private static Task HandleEvent(InstanceEventDtoBase message, AppDbContext appDbContext)
    {
        if (message is InstanceOrderRequestDto orderEv)
        {
            var handler = new CreateInstanceOrderedEvent.Handler(appDbContext);
            return Execute(handler, orderEv);
        }
        else if (message is ItemCancelledEventDto cancelEv)
        {
            var handler = new CreateItemOrderCancelledEvent.Handler(appDbContext);
            return Execute(handler, cancelEv);
        }
        else if (message is ItemDeliveredEventDto deliverEv)
        {
            var handler = new CreateItemDeliveredEvent.Handler(appDbContext);
            return Execute(handler, deliverEv);
        }
        else if (message is ItemDiscardedEventDto discardEv)
        {
            var handler = new CreateItemDiscardedEvent.Handler(appDbContext);
            return Execute(handler, discardEv);
        }
        else
        {
            throw new InvalidOperationException("Unsupported event type");
        }
    }

    private static Task Execute<TRequest>(IAppHandler<TRequest, Unit> handler, TRequest request)
    {
        var validationResult = handler.Validate(request);
        if(validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            return Task.CompletedTask;
        }

        return handler.Handle(request);
    }
}
