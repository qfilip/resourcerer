namespace Resourcerer.DataAccess.Entities;

public class Company : EntityBase
{
    public Company()
    {
        Employees = new HashSet<AppUser>();
        Categories = new HashSet<Category>();
        Instances = new HashSet<Instance>();
    }

    public string? Name { get; set; }
    
    public ICollection<AppUser> Employees { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Instance> Instances { get; set; }
}
