namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : EntityBase
{
    public UnitOfMeasure()
    {
        Excerpts = new HashSet<Excerpt>();
        Elements = new HashSet<Element>();
    }

    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<Element> Elements { get; set; }
}

