﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.V1.Commands;
public static class Register
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
            var existing = await _appDbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Name == request.Name);

            if(existing != null)
            {
                return HandlerResult<AppUserDto>.Rejected("User with the same name already exists");
            }

            var entity = new AppUser
            {
                Name = request.Name,
                PasswordHash = Hasher.GetSha256Hash(request.Password!),
                Permissions = JsonSerializer.Serialize(new Dictionary<string, string>())
            };

            _appDbContext.AppUsers.Add(entity);
            await _appDbContext.SaveChangesAsync();

            var dto = new AppUserDto
            {
                Name = entity.Name,
                Permissions = new()
            };

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
