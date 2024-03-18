using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.Api.Services;

public abstract class EventConsumerServiceBase<TEventBase> : BackgroundService
{
    private readonly IConsumerAdapter<TEventBase> _consumer;
    private readonly IServiceProvider _serviceProvider;
    public EventConsumerServiceBase(IConsumerAdapter<TEventBase> consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!_consumer.IsCompleted())
        {
            var message = await _consumer.ReadAsync();
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

    protected abstract Task HandleEvent(TEventBase message, AppDbContext appDbContext);
}
