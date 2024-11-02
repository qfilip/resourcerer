using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Resourcerer.Messaging.Abstractions;
using Resourcerer.Messaging.Channels;
using Resourcerer.Messaging.Emails.Abstractions;
using Resourcerer.Messaging.Emails.Services;
using System.Threading.Channels;

namespace Resourcerer.Messaging;

public class DependencyInjection
{
    private const string COMMAND_CONSUMER_ENDPOINT = "resourcerer";

    public static void AddMassTransit(
        IServiceCollection services,
        MassTransitConsumersRegistryFunctions[] consumersRegistryFunctions)
    {
        void Loop(Action<MassTransitConsumersRegistryFunctions> action)
        {
            foreach (var crf in consumersRegistryFunctions)
                action(crf);
        }

        services.AddMassTransit(c =>
        {
            Loop(x => x.AddConsumers(c));
            
            c.UsingInMemory((ctx, cfg) =>
            {
                cfg.ReceiveEndpoint(COMMAND_CONSUMER_ENDPOINT, q =>
                {
                    Loop(x => x.ConfigureConsumers(q, ctx));
                });

                Loop(x => x.ConfigureCommands(cfg));
            });

            Loop(x => x.MapCommandEndpoints());
        });
    }

    public static void AddMassTransitSenderService<TMessage, TSender>(IServiceCollection serives) where TSender : class, IMessageSender<TMessage>
    {
        serives.AddSingleton<IMessageSender<TMessage>, TSender>();
    }

    public static void MapCommandEndpoint<T>() where T : class
    {
        EndpointConvention.Map<T>(new Uri($"queue:{COMMAND_CONSUMER_ENDPOINT}"));
    }

    public static void AddChannelMessagingService<TMessage, THostingService>(IServiceCollection services)
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

        services.AddSingleton<IMessageReader<TMessage>, ChannelConsumerService<TMessage>>(sp =>
        {
            var consumer = sp.GetRequiredService<Channel<TMessage>>().Reader;
            return new ChannelConsumerService<TMessage>(consumer);
        });

        services.AddHostedService<THostingService>();
    }

    public static void AddEmailService(IServiceCollection services) =>
        services.AddSingleton<IEmailSender, EmailSender>();
}
