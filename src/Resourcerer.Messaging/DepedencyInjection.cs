﻿using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;
using Resourcerer.Messaging.Emails;
using Resourcerer.Messaging.Emails.Abstractions;
using System.Threading.Channels;

namespace Resourcerer.Messaging;

public static class DepedencyInjection
{
    public static void AddChannelMessagingService<TMessage, THostingService>(this IServiceCollection services)
        where THostingService : ChannelConsumerHostingService<TMessage>
    {
        services.AddSingleton(_ => Channel.CreateUnbounded<TMessage>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<TMessage>>().Writer);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<TMessage>>().Reader);

        services.AddSingleton<IMessageSender<TMessage>, ChannelSenderService<TMessage>>(sp =>
        {
            var sender = sp.GetRequiredService<Channel<TMessage>>().Writer;
            return new ChannelSenderService<TMessage>(sender);
        });

        services.AddSingleton<IMessageConsumer<TMessage>, ChannelConsumerService<TMessage>>(sp =>
        {
            var consumer = sp.GetRequiredService<Channel<TMessage>>().Reader;
            return new ChannelConsumerService<TMessage>(consumer);
        });

        services.AddHostedService<THostingService>();
    }

    public static void AddInMemoryMassTransitMessaging(
        this IServiceCollection services,
        Action<IServiceCollectionBusConfigurator> addConsumers,
        Action<IInMemoryReceiveEndpointConfigurator> configureConsumers,
        Action<IInMemoryBusFactoryConfigurator> addCommandSenders,
        Action mapCommandEndpoints)
    {
        services.AddMassTransit(c =>
        {
            addConsumers.Invoke(c);
            c.UsingInMemory((context, cfg) =>
            {
                cfg.ReceiveEndpoint("", q =>
                {
                    configureConsumers.Invoke(q);
                });

                // cfg.Send<CommandMessageType>();
                addCommandSenders.Invoke(cfg);
            });

            // EndpointConvention.Map<CommandMessageType>(new Uri("uri"));
            mapCommandEndpoints();
        });
    }

    public static void AddEmailMessagingServices(IServiceCollection services)
    {
        services.AddSingleton<IEmailSender, EmailService>();
    }
}
