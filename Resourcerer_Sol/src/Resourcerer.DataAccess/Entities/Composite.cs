namespace Resourcerer.DataAccess.Entities;

public class Composite : EntityBase
{
    public Composite()
    {
        Prices = new HashSet<Price>();
        Excerpts = new HashSet<Excerpt>();
        CompositeInstances = new HashSet<Instance>();
        CompositeSoldEvents = new HashSet<CompositeSoldEvent>();
    }

    public string? Name { get; set; }

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public Guid UnitOfMeasureId { get;set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }

    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<Instance> CompositeInstances { get; set; }
    public ICollection<CompositeSoldEvent> CompositeSoldEvents { get; set; }
}

