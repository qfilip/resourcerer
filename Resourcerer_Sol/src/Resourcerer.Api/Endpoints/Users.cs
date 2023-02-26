using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Resourcerer.Dtos.Users;
using Resourcerer.Logic.Users.Commands;
using System.Security.Claims;

namespace Resourcerer.Api.Endpoints;
public class Users
{
    private static async Task<string>? Login(UserDto dto, IMediator mediatr)
    {
        var claims = await mediatr.Send(new Login.Command(dto));
        
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Issuer = AppStaticData.Jwt.Issuer,
            Audience = AppStaticData.Jwt.Audience,
            Subject =
                new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
            SigningCredentials =
                new SigningCredentials(AppStaticData.Jwt.Key, SecurityAlgorithms.HmacSha256),
        });

        return token;
    }

    private static async Task<IResult> Register(UserDto dto, IMediator mediatr)
    {
        var result = await mediatr.Send(new Register.Command(dto));
        return Results.Ok();
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapPost("users/login", Login);
        app.MapPost("users/register", Register);
    }
}

