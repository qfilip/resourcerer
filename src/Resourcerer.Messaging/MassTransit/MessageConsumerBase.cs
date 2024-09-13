using MassTransit;
using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.Messaging.MassTransit;

public abstract class MessageConsumerBase<T> : IConsumer<T> where T : class
{
    protected readonly AppDbContext _dbContext;
    public MessageConsumerBase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public abstract Task Consume(ConsumeContext<T> context);
}
