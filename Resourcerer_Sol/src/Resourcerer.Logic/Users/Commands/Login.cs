using Resourcerer.Dtos.Users;

namespace Resourcerer.Logic.Users.Commands;

public static class Login
{
    public class Handler : IRequestHandler<UserDto, Dictionary<string, string>>
    {
        public Task<HandlerResult<Dictionary<string, string>>> Handle(UserDto request)
        {
            var claims = new Dictionary<string, string>()
            {
                { "sub", Guid.NewGuid().ToString() },
                { "name", request.Name! },
                { "admin", "" },
                { "elements", "" }
            };
            
            return Task.FromResult(HandlerResult<Dictionary<string, string>>.Ok(claims));
        }
    }
}

