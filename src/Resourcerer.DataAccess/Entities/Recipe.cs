using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Recipe : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable
{
    public Recipe()
    {
        RecipeExcerpts = new HashSet<RecipeExcerpt>();
    }

    public int Version { get; set; }
    
    // relational
    public Guid CompositeItemId { get; set; }
    public Item? CompositeItem { get; set; }

    public ICollection<RecipeExcerpt> RecipeExcerpts { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public Audit AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}
