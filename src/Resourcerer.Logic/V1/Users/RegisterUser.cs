using FluentValidation;
using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1.Users;
using Resourcerer.Utilities;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.V1;

public static class RegisterUser
{
    public class Handler : IAppHandler<V1RegisterUser, AppUserDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IEmailService _emailService;

        public Handler(AppDbContext dbContext, Validator validator, IEmailService emailService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _emailService = emailService;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(V1RegisterUser request)
        {
            var errors = Permissions.Validate(request.PermissionsMap);
            if (_emailService.Validate(request.Email!))
                errors.Add("Invalid email address");

            if(errors.Any())
            {
                return HandlerResult<AppUserDto>.Rejected(errors);
            }

            var company = _dbContext.Companies
                .FirstOrDefault(x => x.Id == request.CompanyId);

            if(company == null)
            {
                return HandlerResult<AppUserDto>.Rejected("Company not found");
            }

            var temporaryPassword = PasswordGenerator.Generate();

            var user = new AppUser()
            {
                Id = Guid.NewGuid(),
                Name = request.Username,
                DisplayName = request.Username,
                Email = request.Email,
                PasswordHash = Hasher.GetSha256Hash(temporaryPassword),
                Permissions = JsonSerializer.Serialize(Permissions.GetCompressedFrom(request.PermissionsMap)),
                
                CompanyId = request.CompanyId
            };

            await _dbContext.SaveChangesAsync();

            var content = $"Hello, you've been added to the system. Username {request.Username}, Password: {temporaryPassword}";
            await _emailService.Send(content, request.Email!);
            
            var result = new AppUserDto
            {
                Id = user.Id,
                Name = user.Name,
                DisplayName= user.DisplayName,
                Email = user.Email,
                PermissionsMap = request.PermissionsMap
            };

            return HandlerResult<AppUserDto>.Ok(result);
        }

        public ValidationResult Validate(V1RegisterUser request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1RegisterUser>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company id cannot be empty");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username cannot be empty");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty");

            RuleFor(x => x.PermissionsMap)
                .NotEmpty().WithMessage("At least one permission must be assigned");
        }
    }
}
