using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class Company : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable
{
    public Company()
    {
        Employees = new HashSet<AppUser>();
        Categories = new HashSet<Category>();
        Instances = new HashSet<Instance>();
        UnitsOfMeasure = new HashSet<UnitOfMeasure>();
    }

    public string? Name { get; set; }

    // relational
    public ICollection<AppUser> Employees { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Instance> Instances { get; set; }
    public ICollection<UnitOfMeasure> UnitsOfMeasure { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public Audit AuditRecord { get; set; } = new();
    public eEntityStatus EntityStatus { get; set; }
}
