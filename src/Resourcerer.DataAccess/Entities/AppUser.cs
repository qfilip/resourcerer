namespace Resourcerer.DataAccess.Entities;
public class AppUser : AppDbEntity
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public string? PasswordHash { get; set; }
    public string? Permissions { get; set; }

    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }
}