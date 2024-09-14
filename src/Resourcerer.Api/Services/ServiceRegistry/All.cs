namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAllAppServices(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {   
        AddAppHandlersAndValidators(services);
        Add3rdParyServices(services, environment);
        AddAuth(services, AppStaticData.Auth.Enabled);
        AddMessagingServices(services, configuration);
        AddEmailServices(services);
    }
}
