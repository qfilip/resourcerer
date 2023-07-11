namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : EntityBase
{
    public UnitOfMeasure()
    {
        Items = new HashSet<Item>();
    }
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public ICollection<Item> Items { get; set; }
}

