using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.Application.Messaging.Emails.Abstractions;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities.Query;
using System.Text.Json;

namespace Resourcerer.Logic.V1;

public static class EditUser
{
    public class Handler : IAppHandler<V1EditUser, AppUserDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IEmailSender _emailSender;
        private readonly IAppIdentityService<AppUser> _identityService;

        public Handler(
            AppDbContext dbContext,
            Validator validator,
            IEmailSender emailSender,
            IAppIdentityService<AppUser> identityService)
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

            if (!_identityService.Get().IsAdmin && entity.IsAdmin)
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
                .NotEmpty().WithMessage("At least one permission must be assigned");
        }
    }
}
