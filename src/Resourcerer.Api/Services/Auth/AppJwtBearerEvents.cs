using Microsoft.AspNetCore.Authentication.JwtBearer;
using Resourcerer.DataAccess.AuthService;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Api.Services.Auth;

public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly AppDbIdentity _appDbIdentity;

    public AppJwtBearerEvents(AppDbIdentity appDbIdentity)
    {
        _appDbIdentity = appDbIdentity;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        foreach(var c in context.Principal.Claims)
        {
            Console.WriteLine(c.Value);
        }

        var idClaimType = AppStaticData.Auth.Jwt.UserId;
        if (context.Principal.Claims.Any(x => x.Type == idClaimType))
        {
            var claim = context.Principal.Claims.First(x => x.Type == idClaimType);
            var userId = Guid.Parse(claim.Value);
            _appDbIdentity.SetUser(new AppUser
            {
                Id = userId
            }); 
        }

        return Task.CompletedTask;
    }
}
