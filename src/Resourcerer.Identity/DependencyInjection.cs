using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Services;
using Resourcerer.Utilities;
using System.Text;

namespace Resourcerer.Identity;

public static class DependencyInjection
{
    private static AppIdentity SystemIdentity = new(Guid.Empty, "system", "system@email.com", true, Guid.Empty);

    public static bool RegisterServices(WebApplicationBuilder builder)
    {
        var section = ConfigurationReader.LoadSection(builder.Configuration, "Auth");
        var authEnabled = ConfigurationReader.Load<bool>(section, "Enabled");

        if (authEnabled)
        {
            builder.Services.AddScoped<AppJwtBearerEventsService>(sp =>
            {
                var identityService = sp.GetRequiredService<IAppIdentityService<AppIdentity>>();
                return new AppJwtBearerEventsService(identityService);
            });

            var secretKey = ConfigurationReader.Load<string>(section, "SecretKey");
            var issuer = ConfigurationReader.Load<string>(section, "Issuer");
            var audience = ConfigurationReader.Load<string>(section, "Audience");
            var tets = ConfigurationReader.LoadValidated<byte>(section, "TokenExpirationTimeSeconds", x => x >= 60);
            var skey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            builder.Services.AddScoped<JwtTokenService>(_ => new JwtTokenService(skey, issuer, audience, tets));

            var jwtScheme = JwtBearerDefaults.AuthenticationScheme;
            builder.Services.AddAuthentication(jwtScheme)
                .AddJwtBearer(jwtScheme, o =>
                {
                    var tvp = new TokenValidationParameters();
                    tvp.ValidIssuer = issuer;
                    tvp.ValidAudience = audience;
                    tvp.IssuerSigningKey = skey;
                    tvp.ValidateIssuer = true;
                    tvp.ValidateAudience = true;
                    tvp.ValidateLifetime = true;
                    tvp.ValidateIssuerSigningKey = true;

                    o.TokenValidationParameters = tvp;
                    o.EventsType = typeof(AppJwtBearerEventsService);
                });

            builder.Services.AddAuthorization(conf =>
                conf.AddPolicy("jwt_policy", b =>
                    b.RequireAuthenticatedUser().AddAuthenticationSchemes(jwtScheme)));
        }

        builder.Services.AddScoped<IAppIdentityService<AppIdentity>, AppIdentityService>(_ =>
            new AppIdentityService(authEnabled, SystemIdentity));

        return authEnabled;
    }
}
