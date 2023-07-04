namespace Resourcerer.DataAccess.Entities;

public class ElementInstance : EntityBase
{
    public double Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }

    public Guid ElementId { get; set; }
    public virtual Element? Element { get; set; }
}
