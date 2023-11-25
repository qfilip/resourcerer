namespace Resourcerer.DataAccess.Entities;

public class Price : EntityBase
{
    public double UnitValue { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }
}
