using FluentValidation;
using System.Security.Claims;

namespace Resourcerer.Dtos.Users;
public class AppUserDto : EntityDto
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public List<Claim>? Claims { get; set; }
}

public class AppUserDtoValidator : AbstractValidator<AppUserDto>
{
    public AppUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("User name cannot be empty");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("User password cannot be empty");
    }
}

