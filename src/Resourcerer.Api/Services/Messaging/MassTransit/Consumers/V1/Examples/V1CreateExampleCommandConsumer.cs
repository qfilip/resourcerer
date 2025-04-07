using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Examples;

public class V1CreateExampleCommandConsumer : ConsumerBase<V1CreateExampleCommand>
{
    public V1CreateExampleCommandConsumer(CreateExample.Handler handler)
        : base(handler.Handle) { }
}
