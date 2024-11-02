using MassTransit;

namespace Resourcerer.Messaging;

public record MassTransitConsumersRegistryFunctions(
    Action<IBusRegistrationConfigurator> AddConsumers,
    Action<IReceiveEndpointConfigurator, IBusRegistrationContext> ConfigureConsumers,
    Action<IInMemoryBusFactoryConfigurator> ConfigureCommands,
    Action MapCommandEndpoints
);