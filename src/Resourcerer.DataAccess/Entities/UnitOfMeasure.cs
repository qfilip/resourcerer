using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : IPkey<Guid>, IAuditedEntity, ISoftDeletable
{
    public UnitOfMeasure()
    {
        Items = new HashSet<Item>();
    }
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    // relational
    public Guid CompanyId { get; set; }
    public virtual Company? Company { get; set; }

    public ICollection<Item> Items { get; set; }


    // entity definition
    public Guid Id { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}

