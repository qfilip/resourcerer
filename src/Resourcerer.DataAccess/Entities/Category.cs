namespace Resourcerer.DataAccess.Entities;

public class Category : AppDbEntity
{
    public Category()
    {
        ChildCategories = new HashSet<Category>();
        Items = new HashSet<Item>();
    }

    public string? Name { get; set; }

    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }

    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    
    public ICollection<Category> ChildCategories { get; set; }
    public ICollection<Item> Items { get; set; }
}
