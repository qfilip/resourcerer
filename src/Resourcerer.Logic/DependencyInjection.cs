using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;

namespace Resourcerer.Logic;

public static class DependencyInjection
{
    public static void RegisterServices(WebApplicationBuilder builder)
    {
        AddMapsterLib(builder.Services);

        var handlerTypes = new Type[] { typeof(IAppHandler<,>), typeof(IAppEventHandler<,>) };
        foreach (var handlerType in handlerTypes)
        {
            RegisterHandlers(handlerType, builder.Services);
            RegisterValidators(builder.Services);
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
        var assembly = typeof(IAssemblyMarker).Assembly;

        var serviceTypes = assembly
        .GetTypes()
        .Where(x =>
            x.GetInterface(interfaceName) != null &&
            !x.IsAbstract &&
            !x.IsInterface)
        .ToList();

        serviceTypes.ForEach(registerAction);
    }

    public static TypeAdapterConfig GetMapsterConfig()
    {
        // mapster
        // check Mapster.Tool package
        var mapsterConfig = new TypeAdapterConfig();
        void TwoWayMap<TSource, TTarget>()
        {
            // PreserveReference (prevent stack overflow on recursive nodes)
            mapsterConfig!.NewConfig<TSource, TTarget>().PreserveReference(true); ;
            mapsterConfig!.NewConfig<TTarget, TSource>().PreserveReference(true); ;
        }

        TwoWayMap<AppUser, AppUserDto>();
        TwoWayMap<Category, CategoryDto>();
        TwoWayMap<Company, CompanyDto>();
        TwoWayMap<Recipe, RecipeDto>();
        TwoWayMap<RecipeExcerpt, ExcerptDto>();
        TwoWayMap<Instance, InstanceDto>();
        TwoWayMap<Item, ItemDto>();
        TwoWayMap<Price, PriceDto>();
        TwoWayMap<UnitOfMeasure, UnitOfMeasureDto>();

        mapsterConfig.Compile(failFast: true);

        return mapsterConfig;
    }

    private static void AddMapsterLib(IServiceCollection services)
    {
        var mapsterConfig = GetMapsterConfig();
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, Mapper>();
    }
}
