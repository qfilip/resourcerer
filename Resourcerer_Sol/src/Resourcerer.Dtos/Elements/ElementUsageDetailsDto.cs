namespace Resourcerer.Dtos.Elements;

public class ElementUsageDetailsDto : BaseDto
{
    public Guid ElementId { get; set; }
    public string? ElementName { get; set; }
    public string? Unit { get; set; }
    public double UnitsPurchased { get; set; }
    public double PurchaseCosts { get; set; }
    public double UnitsUsed { get; set; }
    public double UnitsInStock { get; set; }
}
