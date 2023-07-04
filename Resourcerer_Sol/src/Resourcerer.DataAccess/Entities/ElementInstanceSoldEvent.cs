namespace Resourcerer.DataAccess.Entities;

public class ElementInstanceSoldEvent : EntityBase
{
    public double UnitsSold { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid ElementInstanceId { get; set; }
    public virtual Instance? ElementInstance { get; set; }
}
