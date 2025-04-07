using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1;
using Resourcerer.Messaging.MassTransit;

namespace Resourcerer.Api.Services.Messaging.MassTransit.Consumers.V1.Examples;

public class V1UpdateExampleCommandConsumer : ConsumerBase<V1UpdateExampleCommand>
{
    public V1UpdateExampleCommandConsumer(UpdateExample.Handler handler)
        : base(handler.Handle) { }
}
