using Microsoft.AspNetCore.Authentication.JwtBearer;
using Resourcerer.Application.Abstractions.Services;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Api.Services.Auth;

public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly IAppIdentityService<AppUser> _appIdentityService;

    public AppJwtBearerEvents(IAppIdentityService<AppUser> appIdentityService)
    {
        _appIdentityService = appIdentityService;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        foreach(var c in context.Principal!.Claims)
        {
            Console.WriteLine(c.Value);
        }

        _appIdentityService.Set(context.Principal.Claims);

        return Task.CompletedTask;
    }
}
