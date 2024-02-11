namespace Resourcerer.DataAccess.Entities;

public class Company : EntityBase
{
    public Company()
    {
        Employees = new HashSet<AppUser>();
        Instances = new HashSet<Instance>();
    }

    public string? Name { get; set; }
    public ICollection<AppUser> Employees { get; set; }

    public ICollection<Instance> Instances { get; set; }
}
