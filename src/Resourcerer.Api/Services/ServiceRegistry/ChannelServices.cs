using Resourcerer.Api.Services.V1;
using Resourcerer.Application.Messaging;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddChannelMessagingServices(IServiceCollection services)
    {
        DependencyInjection.AddChannelMessagingService<V1InstanceOrderEvent, InstanceOrderEventService>(services);
        DependencyInjection.AddChannelMessagingService<V1InstanceDiscardedRequest, InstanceDiscardEventService>(services);
        DependencyInjection.AddChannelMessagingService<V1ItemProductionEvent, ItemProductionOrderEventService>(services);
    }
}
