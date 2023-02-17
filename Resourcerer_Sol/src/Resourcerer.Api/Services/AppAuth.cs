using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Resourcerer.Api.Services;
public static partial class ServiceRegistry
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
            {
                Name = "Swagger Auth",
                Type = SecuritySchemeType.ApiKey,
                Scheme = AppStaticData.AuthPolicy.Admin,
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
        });
    }

    public static void AddAuth(this IServiceCollection services)
    {
        var jwtScheme = "jwt";
        
        services.AddAuthentication(jwtScheme)
            .AddJwtBearer(jwtScheme, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = AppStaticData.Jwt.Issuer,
                    ValidAudience = AppStaticData.Jwt.Audience,
                    IssuerSigningKey = AppStaticData.Jwt.Key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

        var admin = AppStaticData.AuthPolicy.Admin;
        services.AddAuthorization(cfg =>
        {
            cfg.AddPolicy(admin, b =>
                b.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(jwtScheme)
                    .RequireClaim(admin));
        });
    }
}

