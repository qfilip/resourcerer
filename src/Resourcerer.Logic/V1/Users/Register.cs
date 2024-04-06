using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.V1.Users;
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

            var user = await _appDbContext.AppUsers
                .Select(x => x.Name)
                .FirstOrDefaultAsync(x => x == request.Username);

            var errors = new List<string>();
            
            if (company != null) errors.Add("Company with the same name already exists");
            if (user != null) errors.Add("Errors with the same name already exists");

            if(errors.Count > 0)
            {
                return HandlerResult<AppUserDto>.Rejected(errors);
            }

            var adminUser = new AppUser
            {
                Id = Guid.NewGuid(),
                Name = request.Username,
                IsAdmin = true,
                PasswordHash = Hasher.GetSha256Hash(request.Password!),
                Permissions = JsonSerializer.Serialize(Permissions.GetCompressed())
            };

            var newCompany = new Company
            {
                Name = request.CompanyName,
                Employees = [ adminUser ]
            };

            _appDbContext.Companies.Add(newCompany);
            await _appDbContext.SaveChangesAsync();

            var result = await _appDbContext.AppUsers
                .Include(x => x.Company)
                .FirstAsync(x => x.Id == adminUser.Id);

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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("User password cannot be empty");

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("Company name cannot be empty");
        }
    }
}
