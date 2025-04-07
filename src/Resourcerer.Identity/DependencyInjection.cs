using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Services;

namespace Resourcerer.Identity;

public static class DependencyInjection
{
    private static AppIdentity SystemIdentity = new(Guid.Empty, "system", "system@email.com");
    public static void AddAppIdentityServices(
        IServiceCollection services,
        bool authEnabled,
        SymmetricSecurityKey secretKey,
        string issuer,
        string audience,
        string authorizationPolicyName)
    {
        services.AddTransient(_ => new JwtTokenService(secretKey, issuer, audience));
        services.AddScoped<AppJwtBearerEventsService>();
        services.AddScoped<IAppIdentityService<AppIdentity>, AppIdentityService>(_ =>
            new AppIdentityService(authEnabled, SystemIdentity));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = secretKey,
                
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            o.EventsType = typeof(AppJwtBearerEventsService);
        });

        services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy(authorizationPolicyName, b =>
                b.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
        });
    }
}
