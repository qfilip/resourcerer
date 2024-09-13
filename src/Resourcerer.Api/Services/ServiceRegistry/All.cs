namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAllAppServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        AddAppHandlersAndValidators(services);
        Add3rdParyServices(services, environment);
        AddAuth(services);
        AddChannelMessagingServices(services);
        AddEmailServices(services);
    }
}
