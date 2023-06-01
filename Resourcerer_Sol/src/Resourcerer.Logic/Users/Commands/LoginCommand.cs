using MediatR;
using Resourcerer.Dtos.Users;

namespace Resourcerer.Logic.Users.Commands;

public static class Login
{
    public class Command : IRequest<Dictionary<string, string>>
    {
        public Command(UserDto dto)
        {
            Dto = dto;
        }

        public UserDto Dto { get; }
    }

    public class Handler : IRequestHandler<Command, Dictionary<string, string>>
    {
        public Task<Dictionary<string, string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var claims = new Dictionary<string, string>()
            {
                { "sub", Guid.NewGuid().ToString() },
                { "name", request.Dto.Name! },
                { "admin", "" }
            };
            
            return Task.FromResult(claims);
        }
    }
}

