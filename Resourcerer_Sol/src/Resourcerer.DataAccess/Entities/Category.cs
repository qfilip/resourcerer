namespace Resourcerer.DataAccess.Entities;

public class Category : EntityBase
{
    public Category()
    {
        ChildCategories = new HashSet<Category>();
        Composites = new HashSet<Composite>();
        Elements = new HashSet<Element>();
    }

    public string? Name { get; set; }
    
    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    
    public ICollection<Category> ChildCategories { get; set; }
    public ICollection<Composite> Composites { get; set; }
    public ICollection<Element> Elements { get; set; }
}
