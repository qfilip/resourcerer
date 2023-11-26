using Resourcerer.DataAccess.Contexts;
using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class ItemEventHandler : BackgroundService
{
    private readonly ChannelReader<int> _reader;
    private readonly IServiceProvider _serviceProvider;

    public ItemEventHandler(
        ChannelReader<int> reader,
        IServiceProvider serviceProvider)
    {
        _reader = reader;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!_reader.Completion.IsCompleted)
        {
            var message = await _reader.ReadAsync();
            using (var scope = _serviceProvider.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if(database == null)
                {
                    throw new InvalidOperationException("Failed to resolve database service");
                }


            }
        }
    }
}
