using FluentValidation;
using System.Security.Claims;

namespace Resourcerer.Dtos;
public class AppUserDto : BaseDto<AppUserDto>
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public Dictionary<string, int>? Permissions { get; set; }

    public override AbstractValidator<AppUserDto> GetValidator() => new Validator();

    private class Validator : AbstractValidator<AppUserDto>
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



