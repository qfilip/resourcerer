namespace Resourcerer.DataAccess.Entities;

public class Price : EntityBase
{
    public DateTime ValidFrom { get; set; }
    public decimal Value { get; set; }

    public virtual Element? Element { get; set; }
    public virtual Composite? Composite { get; set; }
}