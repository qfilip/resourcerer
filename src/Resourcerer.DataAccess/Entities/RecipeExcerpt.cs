using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class RecipeExcerpt : IAuditedEntity<Audit>, ISoftDeletable
{
    public double Quantity { get; set; }

    // relational
    public Guid RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    
    public  Guid ElementId { get; set; }
    public Item? Element { get; set; }

    // entity definition
    public Audit AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}

