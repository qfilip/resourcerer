using System.Threading.Channels;

namespace Resourcerer.Api.Services.V1_0;

public class ItemEventHandler : BackgroundService
{
    private readonly ChannelReader<int> _reader;

    public ItemEventHandler(ChannelReader<int> reader)
    {
        _reader = reader;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tcs = new TaskCompletionSource<int>();
        throw new NotImplementedException();
    }
}
