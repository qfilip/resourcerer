using Resourcerer.DataAccess.Contexts;
using System.Threading.Channels;

namespace Resourcerer.Api.Services;

public abstract class EventServiceBase<TEventBase> : BackgroundService
{
    private readonly ChannelReader<TEventBase> _reader;
    private readonly IServiceProvider _serviceProvider;
    public EventServiceBase(ChannelReader<TEventBase> reader, IServiceProvider serviceProvider)
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

    protected abstract Task HandleEvent(TEventBase message, AppDbContext appDbContext);
}
