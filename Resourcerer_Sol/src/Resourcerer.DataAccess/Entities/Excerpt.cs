using System.Xml.Linq;

namespace Resourcerer.DataAccess.Entities;

public class Excerpt : EntityBase
{
    public Guid CompositeId { get; set; }
    public virtual Composite? Composite { get; set; }
    
    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasure? UnitOfMeasure { get; set; }

    public int Quantity { get; set; }
}

