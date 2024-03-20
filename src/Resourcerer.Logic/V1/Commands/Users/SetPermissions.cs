using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using System.Text.Json;

namespace Resourcerer.Logic.V1.Commands;

public static class SetPermissions
{
    public class Handler : IAppHandler<V1SetUserPermissions, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(V1SetUserPermissions request)
        {
            var errors = Permissions.Validate(request.Permissions!);
            if(errors.Any())
            {
                return HandlerResult<AppUserDto>.Rejected(errors.ToArray());
            }

            var user = await _appDbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if(user == null)
            {
                return HandlerResult<AppUserDto>.NotFound($"User with Id {request.UserId} not found");
            }

            user.Permissions = JsonSerializer.Serialize(request.Permissions);

            await _appDbContext.SaveChangesAsync();

            var dto = new AppUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Permissions = Permissions.GetPermissionDictFromString(user.Permissions!)
            };

            return HandlerResult<AppUserDto>.Ok(dto);
        }

        public ValidationResult Validate(V1SetUserPermissions request) => new Validator().Validate(request);

        public class Validator : AbstractValidator<V1SetUserPermissions>
        {
            public Validator()
            {
                RuleFor(x => x.Permissions)
                    .NotEmpty()
                    .WithMessage("User id cannot be empty");

                RuleFor(x => x.Permissions)
                    .NotNull()
                    .WithMessage("Permissions cannot be null");
            }
        }
    }
}
