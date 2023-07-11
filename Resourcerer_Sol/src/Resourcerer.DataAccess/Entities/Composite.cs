namespace Resourcerer.DataAccess.Entities;

public class Composite : EntityBase
{
    public Guid CompositeId { get; set; }
    public virtual Item? CompositeItem { get; set; }

    public ICollection <Excerpt>? Excerpts { get; set; }
}
