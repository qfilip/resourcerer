using MassTransit;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(c =>
        {
            //c.AddConsumer<>();

            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ReceiveEndpoint("", q =>
                {
                    // q.ConfigureConsumer<>(ctx);
                });

                // cfg.Send<Command>();
            });

            // EndpointConvention.Map<Command>(new Uri(""));
        });
    }
}
