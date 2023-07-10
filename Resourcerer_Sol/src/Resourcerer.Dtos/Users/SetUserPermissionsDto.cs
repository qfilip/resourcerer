using FluentValidation;

namespace Resourcerer.Dtos;

public class SetUserPermissionsDto : IBaseDto<SetUserPermissionsDto>
{
    public Guid UserId { get; set; }
    public Dictionary<string, string>? Permissions { get; set; }

    public AbstractValidator<SetUserPermissionsDto> GetValidator() => new Validator();

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
