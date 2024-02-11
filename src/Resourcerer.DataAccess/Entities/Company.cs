namespace Resourcerer.DataAccess.Entities;

public class Company : EntityBase
{
    public Company()
    {
        Employees = new HashSet<AppUser>();
    }

    public string? Name { get; set; }
    public ICollection<AppUser> Employees { get; set; }
}
