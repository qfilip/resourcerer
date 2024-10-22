using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Excerpt : IAuditedEntity<Audit>, ISoftDeletable
{
    public double Quantity { get; set; }

    // relational
    public Guid CompositeId { get; set; }
    public Item? Composite { get; set; }
    
    public  Guid ElementId { get; set; }
    public Item? Element { get; set; }

    // entity definition
    public Audit AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}

