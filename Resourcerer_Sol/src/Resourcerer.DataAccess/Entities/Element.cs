namespace Resourcerer.DataAccess.Entities;

public class Element : EntityBase
{
    public Element()
    {
        Excerpts = new HashSet<Excerpt>();
    }

    public string? Name { get; set; }

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public Guid CurrentPriceId { get; set; }
    public virtual Price? CurrentPrice { get; set; }

    public ICollection<Excerpt> Excerpts { get; set; }
}

