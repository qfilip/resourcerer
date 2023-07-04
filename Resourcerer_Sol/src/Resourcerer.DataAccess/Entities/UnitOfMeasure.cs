namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : EntityBase
{
    public UnitOfMeasure()
    {
        Elements = new HashSet<Element>();
        Composites = new HashSet<Composite>();
    }
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public ICollection<Element> Elements { get; set; }
    public ICollection<Composite> Composites { get; set; }
}

