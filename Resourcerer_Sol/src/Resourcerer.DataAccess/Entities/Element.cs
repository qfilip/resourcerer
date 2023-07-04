namespace Resourcerer.DataAccess.Entities;

public class Element : EntityBase
{
    public Element()
    {
        Excerpts = new HashSet<Excerpt>();
        Prices = new HashSet<Price>();
        ElementInstances = new HashSet<ElementInstance>();
    }

    public string? Name { get; set; }
    
    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }
    
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<ElementInstance> ElementInstances { get; set; }
}

