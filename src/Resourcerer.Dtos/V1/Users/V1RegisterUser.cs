namespace Resourcerer.Dtos.V1.Users;

public class V1RegisterUser : IDto
{
    public Guid CompanyId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public Dictionary<string, string[]> PermissionsMap { get; set; } = new();
}
