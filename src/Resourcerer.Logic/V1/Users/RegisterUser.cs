﻿using FluentValidation;
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

        public async Task<HandlerResult<AppUserDto>> Handle(V1RegisterUser request)
        {
            if(!_identityService.Get().IsAdmin && request.IsAdmin)
                return HandlerResult<AppUserDto>.Rejected("Only admin can add another admin user");
            
            var errors = Permissions.Validate(request.PermissionsMap);
            
            if (!_emailSender.Validate(request.Email!))
                errors.Add("Invalid email address");

            if (errors.Any())
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
                IsAdmin = request.IsAdmin,
                Email = request.Email,
                PasswordHash = Hasher.GetSha256Hash(temporaryPassword),
                Permissions = JsonSerializer.Serialize(Permissions.GetCompressedFrom(request.PermissionsMap)),
                
                CompanyId = request.CompanyId
            };

            _dbContext.AppUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            var content = $"Hello, you've been added to the system. Username {request.Username}, Password: {temporaryPassword}";
            
            await _emailSender.SendAsync(new(user.Email, "Welcome to Resourcerer", content));
            
            var result = await _dbContext.AppUsers
                .Where(x => x.Id == user.Id)
                .Select(AppUsers.DefaultDtoProjection)
                .FirstAsync();

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
