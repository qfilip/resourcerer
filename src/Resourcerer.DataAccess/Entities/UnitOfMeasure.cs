namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : EntityBase
{
    public UnitOfMeasure()
    {
        Items = new HashSet<Item>();
    }
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    // relational
    public Guid? CompanyId { get; set; }
    public virtual Company? Company { get; set; }

    public ICollection<Item> Items { get; set; }
}

