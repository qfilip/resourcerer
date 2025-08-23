using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Services;

namespace Resourcerer.Identity;

public static class DependencyInjection
{
    private static AppIdentity SystemIdentity = new(Guid.Empty, "system", "system@email.com");
    public static void RegisterServices(
        WebApplicationBuilder builder,
        bool authEnabled,
        SymmetricSecurityKey? secretKey = null,
        string? issuer  = null,
        string? audience = null,
        string? authorizationPolicyName = null)
    {
        if (!authEnabled) return;

        if (secretKey == null)
            throw new ArgumentException("Secret key not supplied");

        if (issuer == null)
            throw new ArgumentException("Issuer not supplied");

        if (audience == null)
            throw new ArgumentException("Audience not supplied");

        if (authorizationPolicyName == null)
            throw new ArgumentException("Authorization Policy Name not supplied");

        var services = builder.Services;

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
