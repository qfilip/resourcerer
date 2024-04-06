using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.Logic.V1.Users;

public static class Login
{
    public class Handler : IAppHandler<AppUserDto, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(AppUserDto request)
        {
            var users = await _appDbContext.AppUsers
                .Include(x => x.Company)
                .Where(x => x.Name == request.Name)
                .ToArrayAsync();

            if (users.Length == 0)
            {
                return HandlerResult<AppUserDto>.NotFound($"Username not found");
            }

            var hash = Hasher.GetSha256Hash(request.Password!);
            var user = users.FirstOrDefault(x => x.PasswordHash == hash);

            if(user == null)
            {
                return HandlerResult<AppUserDto>.Rejected($"Bad credentials");
            }

            var permissionDict = Permissions.GetCompressedFrom(user.Permissions!);

            var dto = AppUserDto.MapForJwt(user);

            return HandlerResult<AppUserDto>.Ok(dto);
        }

        public ValidationResult Validate(AppUserDto request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<AppUserDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("User name cannot be empty");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("User password cannot be empty");
        }
    }
}

