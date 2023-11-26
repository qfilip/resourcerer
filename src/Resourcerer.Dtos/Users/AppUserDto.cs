using FluentValidation;

namespace Resourcerer.Dtos;
public class AppUserDto : EntityDto<AppUserDto>
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public Dictionary<string, int>? Permissions { get; set; }
}



