using Microsoft.AspNetCore.Authentication.JwtBearer;
using Resourcerer.Application.Services;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Api.Services.Auth;

public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly AppIdentityService<AppUser> _appIdentityService;

    public AppJwtBearerEvents(AppIdentityService<AppUser> appIdentityService)
    {
        _appIdentityService = appIdentityService;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        foreach(var c in context.Principal!.Claims)
        {
            Console.WriteLine(c.Value);
        }

        var idClaimType = AppStaticData.Auth.Jwt.UserId;
        if (context.Principal.Claims.Any(x => x.Type == idClaimType))
        {
            var claim = context.Principal.Claims.First(x => x.Type == idClaimType);
            var userId = Guid.Parse(claim.Value);
            _appIdentityService.SetUser(new AppUser
            {
                Id = userId
            }); 
        }

        return Task.CompletedTask;
    }
}
