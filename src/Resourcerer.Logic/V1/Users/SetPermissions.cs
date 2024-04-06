using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using System.Text.Json;

namespace Resourcerer.Logic.V1;

public static class SetPermissions
{
    public class Handler : IAppHandler<V1SetUserPermissions, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(V1SetUserPermissions request)
        {
            var errors = Permissions.Validate(request.Permissions!);
            if (errors.Any())
            {
                return HandlerResult<AppUserDto>.Rejected(errors.ToArray());
            }

            var user = await _appDbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
            {
                return HandlerResult<AppUserDto>.NotFound($"User with Id {request.UserId} not found");
            }

            user.Permissions = JsonSerializer.Serialize(Permissions.GetCompressedFrom(request.Permissions!));

            await _appDbContext.SaveChangesAsync();

            var dto = new AppUserDto
            {
                Id = user.Id,
                Name = user.Name,
                PermissionsMap = Permissions.GetPermissionsMap(user.Permissions!)
            };

            return HandlerResult<AppUserDto>.Ok(dto);
        }

        public ValidationResult Validate(V1SetUserPermissions request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<V1SetUserPermissions>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User id cannot be empty");

            RuleFor(x => x.Permissions)
                .NotNull()
                .WithMessage("Permissions cannot be null");
        }
    }
}
