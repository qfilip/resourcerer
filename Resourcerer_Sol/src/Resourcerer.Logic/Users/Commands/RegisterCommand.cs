using MediatR;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Data;
using Resourcerer.Dtos.Users;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Users.Commands;
public static class Register
{
    public class Command : IRequest<ResultData<UserDto>>
    {
        public Command(UserDto dto)
        {
            Dto = dto;
        }

        public UserDto Dto { get; }
    }

    public class Handler : IRequestHandler<Command, ResultData<UserDto>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultData<UserDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.Dto.Name) || string.IsNullOrEmpty(request.Dto.Password))
            {
                return new ResultData<UserDto>(new string[] { "Name or password cannot be empty" });
            }

            var claims = new Dictionary<string, string>()
            {
                { nameof(request.Dto.Name), request.Dto.Name },
                { nameof(request.Dto.Role), request.Dto.Role ?? "" },
            };

            var claimsJson = JsonSerializer.Serialize(claims);

            var user = new AppUser
            {
                Name = request.Dto.Name,
                PasswordHash = Hasher.GetSha256Hash(request.Dto.Password),
                Claims = claimsJson
            };

            _dbContext.AppUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return new ResultData<UserDto>(request.Dto);
        }
    }
}
