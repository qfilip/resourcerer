namespace Resourcerer.DataAccess.Entities;

public class Composite : EntityBase
{
    public Composite()
    {
        Prices = new HashSet<Price>();
        Excerpts = new HashSet<Excerpt>();
        CompositeSoldEvents = new HashSet<CompositeSoldEvent>();
    }

    public string? Name { get; set; }
    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }
    public ICollection<Price> Prices { get; set; }
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<CompositeSoldEvent> CompositeSoldEvents { get; set; }
}

