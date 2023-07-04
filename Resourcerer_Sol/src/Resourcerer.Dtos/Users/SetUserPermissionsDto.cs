using FluentValidation;

namespace Resourcerer.Dtos;

public class SetUserPermissionsDto : BaseDto
{
    public Guid UserId { get; set; }
    public Dictionary<string, string>? Permissions { get; set; }
}

public class SetUserPermissionsDtoValidator : AbstractValidator<SetUserPermissionsDto>
{
    public SetUserPermissionsDtoValidator()
    {
        RuleFor(x => x.Permissions)
            .NotEmpty()
            .WithMessage("User id cannot be empty");

        RuleFor(x => x.Permissions)
            .NotNull()
            .WithMessage("Permissions cannot be null");
    }
}
