using Resourcerer.Api.Services.StaticServices;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAppIdentity(IServiceCollection services)
    {
        services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

        Identity.DependencyInjection.AddAppIdentityServices(
            services,
            AppStaticData.Auth.Enabled,
            AppStaticData.Auth.Jwt.Key!,
            AppStaticData.Auth.Jwt.Issuer,
            AppStaticData.Auth.Jwt.Audience,
            AppStaticData.Auth.AuthorizationPolicy.Jwt
        );
    }
}
