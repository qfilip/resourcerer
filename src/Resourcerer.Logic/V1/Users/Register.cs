using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Utils;
using Resourcerer.Logic.Utilities.Query;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.V1;
public static class Register
{
    public class Handler : IAppHandler<V1Register, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(V1Register request)
        {
            var company = await _appDbContext.Companies
                .Select(x => x.Name)
                .FirstOrDefaultAsync(x => x == request.CompanyName);

            if(company != null)
            {
                return HandlerResult<AppUserDto>.Rejected("Company with the same name already exists");
            }

            var adminUser = new AppUser
            {
                Id = Guid.NewGuid(),
                Name = request.Username,
                DisplayName = request.DisplayName ?? string.Empty,
                Email = request.Email,
                IsAdmin = true,
                PasswordHash = Hasher.GetSha256Hash(request.Password!),
                Permissions = JsonSerializer.Serialize(Permissions.GetCompressed()),
                Company = new Company
                {
                    Name = request.CompanyName
                }
            };

            _appDbContext.AppUsers.Add(adminUser);
            await _appDbContext.SaveChangesAsync();

            var result = await _appDbContext.AppUsers
                .Where(x => x.Id == adminUser.Id)
                .Select(AppUsers.DefaultProjection)
                .FirstAsync();

            return HandlerResult<AppUserDto>.Ok(AppUserDto.MapForJwt(result));
        }

        public ValidationResult Validate(V1Register request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<V1Register>
    {
        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("User name cannot be empty");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email invalid");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("User password cannot be empty");

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("Company name cannot be empty");
        }
    }
}
