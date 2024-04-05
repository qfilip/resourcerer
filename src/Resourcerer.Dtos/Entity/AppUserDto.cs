namespace Resourcerer.Dtos.Entity;
public class AppUserDto : EntityDto<AppUserDto>
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Jwt { get; set; }
    public Dictionary<string, string[]>? PermissionsMap { get; set; }
}



