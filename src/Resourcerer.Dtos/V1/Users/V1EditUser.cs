namespace Resourcerer.Dtos.V1;

public class V1EditUser : IDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public Dictionary<string, string[]> PermissionsMap { get; set; } = new();
}
