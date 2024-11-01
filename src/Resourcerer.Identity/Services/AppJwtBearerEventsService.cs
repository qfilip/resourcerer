using Microsoft.AspNetCore.Authentication.JwtBearer;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.Identity.Services;

public class AppJwtBearerEventsService : JwtBearerEvents
{
    private readonly IAppIdentityService<AppIdentity> _appIdentityService;

    public AppJwtBearerEventsService(IAppIdentityService<AppIdentity> appIdentityService)
    {
        _appIdentityService = appIdentityService;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal == null)
            throw new Exception("User principal not found");

        _appIdentityService.Set(context.Principal.Claims);

        return Task.CompletedTask;
    }
}
