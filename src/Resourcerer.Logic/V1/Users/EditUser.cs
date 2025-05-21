using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.Utilities;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Messaging.Emails.Abstractions;
using System.Text.Json;

namespace Resourcerer.Logic.V1;

public static class EditUser
{
    public class Handler : IAppHandler<V1EditUser, AppUserDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IEmailSender _emailSender;
        private readonly IAppIdentityService<AppIdentity> _identityService;

        public Handler(
            AppDbContext dbContext,
            Validator validator,
            IEmailSender emailSender,
            IAppIdentityService<AppIdentity> identityService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _emailSender = emailSender;
            _identityService = identityService;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(V1EditUser request)
        {
            var errors = Permissions.Validate(request.PermissionsMap);

            if (!_emailSender.Validate(request.Email!))
                errors.Add("Invalid email address");

            if (errors.Any())
            {
                return HandlerResult<AppUserDto>.Rejected(errors);
            }

            var entity = await _dbContext.AppUsers
                .Where(x => x.Id == request.UserId)
                .Select(AppUsers.DefaultProjection)
                .FirstOrDefaultAsync();

            if (entity == null)
                return HandlerResult<AppUserDto>.NotFound($"User with id {request.UserId} not found");

            if (!_identityService.Get().Admin && entity.IsAdmin)
                return HandlerResult<AppUserDto>.Rejected("Only admin can add another admin user");

            if (entity.Company!.Id != _identityService.Get().CompanyId)
                return HandlerResult<AppUserDto>.Rejected($"Editing user from another company is forbidden");

            _dbContext.AppUsers.Attach(entity);

            entity.Email = request.Email;
            entity.IsAdmin = request.IsAdmin;
            entity.Permissions = JsonSerializer.Serialize(Permissions.GetCompressedFrom(request.PermissionsMap));

            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.AppUsers
                .Where(x => x.Id == entity.Id)
                .Select(AppUsers.DefaultDtoProjection)
                .FirstAsync();

            return HandlerResult<AppUserDto>.Ok(result);
        }

        public ValidationResult Validate(V1EditUser request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1EditUser>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id cannot be empty");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty");

            RuleFor(x => x.PermissionsMap)
                .Must(Validation.AppUser.Permissions)
                .WithMessage(Validation.AppUser.PermissionsError);
        }
    }
}
