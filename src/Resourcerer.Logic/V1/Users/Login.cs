using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.Utilities;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.Logic.V1;

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
                .Where(x => x.Name == request.Name)
                .Select(AppUsers.Expand(x => new AppUser
                { 
                    PasswordHash = x.PasswordHash
                }))
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
                .Must(Validation.AppUser.Name)
                .WithMessage(Validation.AppUser.NameError);

            RuleFor(x => x.Password)
                .Must(Validation.AppUser.Password)
                .WithMessage(Validation.AppUser.PasswordError);
        }
    }
}

