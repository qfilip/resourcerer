using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Item : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable
{
    public Item()
    {
        CompositeExcerpts = new HashSet<Excerpt>();
        ElementExcerpts = new HashSet<Excerpt>();
        Prices = new HashSet<Price>();
        Instances = new HashSet<Instance>();
        ProductionOrders = new HashSet<ItemProductionOrder>();
    }

    public string? Name { get; set; }
    public decimal ProductionPrice { get; set; }
    public double ProductionTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }

    // relational

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }

    public ICollection<Excerpt> ElementExcerpts { get; set; }
    public ICollection<Excerpt> CompositeExcerpts { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<Instance> Instances { get; set; }
    public ICollection<ItemProductionOrder> ProductionOrders { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public Audit AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}

