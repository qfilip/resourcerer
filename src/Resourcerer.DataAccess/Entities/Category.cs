using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Category : IPkey<Guid>, IAuditedEntity, ISoftDeletable
{
    public Category()
    {
        ChildCategories = new HashSet<Category>();
        Items = new HashSet<Item>();
    }

    public string? Name { get; set; }

    // relational
    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }

    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }
    
    public ICollection<Category> ChildCategories { get; set; }
    public ICollection<Item> Items { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}
