namespace Resourcerer.DataAccess.Entities;
public class AppUser : EntityBase
{
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
    public string? Permissions { get; set; }

    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }
}