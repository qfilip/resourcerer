using FluentValidation;
using Resourcerer.Application.Abstractions.Handlers;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAppHandlersAndValidators(IServiceCollection services)
    {
        services.AddScoped<Pipeline>();
        
        var handlerTypes = new Type[] { typeof(IAppHandler<,>), typeof(IAppEventHandler<,>) };
        foreach (var handlerType in handlerTypes)
        {
            RegisterHandlers(handlerType, services);
            RegisterValidators(services);
        }
    }

    private static void RegisterHandlers(Type handlerType, IServiceCollection services)
    {
        var interfaceName = handlerType.Name;
        var registerAction = (Type x) =>
        {
            services.AddTransient(x);
        };

        RegisterDynamically(interfaceName, registerAction);
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        var interfaceName = typeof(IValidator).Name;
        var registerAction = (Type x) =>
        {
            services.AddSingleton(x);
        };

        RegisterDynamically(interfaceName, registerAction);
    }

    private static void RegisterDynamically(string interfaceName, Action<Type> registerAction)
    {
        var assembly = typeof(Logic.IAssemblyMarker).Assembly;

        var serviceTypes = assembly
        .GetTypes()
        .Where(x =>
            x.GetInterface(interfaceName) != null &&
            !x.IsAbstract &&
            !x.IsInterface)
        .ToList();

        serviceTypes.ForEach(registerAction);
    }
}
