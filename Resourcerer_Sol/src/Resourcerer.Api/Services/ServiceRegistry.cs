using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic;
using System.Reflection;

namespace Resourcerer.Api.Services;
public static partial class ServiceRegistry
{
    public static void AddAppServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddDbContext<AppDbContext>(cfg =>
            cfg.UseSqlite(AppInitializer.GetDbConnection(env)));
    }

    public static void AddAspNetServices(this IServiceCollection services)
    {
        services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        services.AddAuth();
        services.AddSwagger();
        services.AddAuthorization();
    }

    public static void Add3rdParyServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            var assembly = typeof(IResourcererLogicAssemblyMarker).GetTypeInfo().Assembly;
            cfg.RegisterServicesFromAssembly(assembly);
        });
    }

    private static void AddSwagger(this IServiceCollection services)
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

    private static void AddAuth(this IServiceCollection services)
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

