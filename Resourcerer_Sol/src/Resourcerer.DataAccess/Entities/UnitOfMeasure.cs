namespace Resourcerer.DataAccess.Entities;

public class UnitOfMeasure : EntityBase
{
    public UnitOfMeasure()
    {
        Excerpts = new HashSet<Excerpt>();
    }

    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public ICollection<Excerpt> Excerpts { get; set; }
}

