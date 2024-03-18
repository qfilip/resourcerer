namespace Resourcerer.Api.Services;

public interface ISenderAdapter<TMessage>
{
    public Task SendAsync(TMessage message);
}

public interface IConsumerAdapter<TMessage>
{
    public Task<TMessage> ReadAsync();
    public bool IsCompleted();
}
