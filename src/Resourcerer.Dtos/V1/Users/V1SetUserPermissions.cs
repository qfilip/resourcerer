namespace Resourcerer.Dtos.V1;

public class V1SetUserPermissions : IDto
{
    public Guid UserId { get; set; }
    public Dictionary<string, string[]>? Permissions { get; set; }
}
