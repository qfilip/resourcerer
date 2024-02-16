﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using System.Text.Json;

namespace Resourcerer.Logic.Commands.V1_0;

public static class SetPermissions
{
    public class Handler : IAppHandler<SetUserPermissionsDto, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(SetUserPermissionsDto request)
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

        public ValidationResult Validate(SetUserPermissionsDto request) => new Validator().Validate(request);

        public class Validator : AbstractValidator<SetUserPermissionsDto>
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