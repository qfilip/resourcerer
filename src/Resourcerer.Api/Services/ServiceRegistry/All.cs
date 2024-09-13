namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAllAppServices(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        var useChannelsForMessaging = configuration
            .GetSection("Messaging")
            .GetValue<bool>("UseChannels");
        
        AddAppHandlersAndValidators(services);
        Add3rdParyServices(services, environment);
        AddAuth(services, AppStaticData.Auth.Enabled);
        AddChannelMessagingServices(services, useChannelsForMessaging);
        AddEmailServices(services);
    }
}
