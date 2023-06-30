using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.Users;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Users;
using System.Security.Claims;

namespace Resourcerer.Api.Endpoints.Users;

public class LoginEndpoint
{
    //public static async Task<IResult> Action(
    //   [FromBody] UserDto dto,
    //   [FromServices] Pipeline pipeline,
    //   [FromServices] Login.Handler handler,
    //   [FromServices] UserManager<IdentityUser> userManager)
    //{
    //    //userManager.AddClaimsAsync();
    //    //userManager.SignIn
    //    await pipeline.Pipe(handler, dto, nameof(Login), async (handlerResult) =>
    //    {
    //        if(handlerResult.Status == HandlerResultStatus.Ok)
    //        {
    //            var claims = handlerResult.Object!.Aggregate(new List<Claim>(), (acc, x) =>
    //            {
    //                acc.Add(new Claim(x.Key, x.Value));
    //                return acc;
    //            });
                
    //            var identityUser = new IdentityUser
    //            {
    //                UserName = dto.Name
    //            };

    //            await userManager.CreateAsync(identityUser, password: dto.Password!);
    //            await userManager.AddClaimsAsync(identityUser, claims);
    //        }


    //    });
    //}
}
