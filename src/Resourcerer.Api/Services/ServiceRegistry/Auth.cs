using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Resourcerer.Api.Services.Auth;
using Resourcerer.Application.Auth;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Api.Services;

public static partial class ServiceRegistry
{
    public static void AddAuth(IServiceCollection services, bool authEnabled)
    {
        services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        services.AddScoped<AppJwtBearerEvents>();
        
        services.AddScoped<IAppIdentityService<AppUser>, AppIdentityService>(_ =>
            new AppIdentityService(AppStaticData.Auth.Enabled));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = AppStaticData.Auth.Jwt.Issuer,
                ValidAudience = AppStaticData.Auth.Jwt.Audience,
                IssuerSigningKey = AppStaticData.Auth.Jwt.Key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            o.EventsType = typeof(AppJwtBearerEvents);
        });

        services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy(AppStaticData.Auth.AuthorizationPolicy.Jwt, b =>
                b.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));
        });
    }
}
