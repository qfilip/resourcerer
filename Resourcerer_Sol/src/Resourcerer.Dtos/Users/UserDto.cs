namespace Resourcerer.Dtos.Users;
public class UserDto
{
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
    public List<Permission>? Claims { get; set; }
}

