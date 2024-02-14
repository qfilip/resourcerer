namespace Resourcerer.DataAccess.Entities;

public class Item : EntityBase
{
    public Item()
    {
        CompositeExcerpts = new HashSet<Excerpt>();
        ElementExcerpts = new HashSet<Excerpt>();
        Prices = new HashSet<Price>();
        Instances = new HashSet<Instance>();
    }

    public string? Name { get; set; }
    public double ProductionPrice { get; set; }
    public double ProductionTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }

    public ICollection<Excerpt> ElementExcerpts { get; set; }
    public ICollection<Excerpt> CompositeExcerpts { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<Instance> Instances { get; set; }
}

