namespace Resourcerer.DataAccess.Entities;

public class Company : EntityBase
{
    public Company()
    {
        Employees = new HashSet<AppUser>();
        Items = new HashSet<Item>();
        Instances = new HashSet<Instance>();
    }

    public string? Name { get; set; }
    
    public ICollection<AppUser> Employees { get; set; }
    public ICollection<Item> Items { get; set; }
    public ICollection<Instance> Instances { get; set; }
}
