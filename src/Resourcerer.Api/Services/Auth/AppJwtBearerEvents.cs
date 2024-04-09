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

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        var t = context.Exception.Message;
        return Task.CompletedTask;
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
        var t = context.Token;
        return Task.CompletedTask;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        if(context.Principal == null)
        {
            throw new Exception("User principal not found");
        }

        _appIdentityService.Set(context.Principal.Claims);

        return Task.CompletedTask;
    }
}
