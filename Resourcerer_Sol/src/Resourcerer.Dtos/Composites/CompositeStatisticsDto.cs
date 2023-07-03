namespace Resourcerer.Dtos.Composites;

public class CompositeStatisticsDto : BaseDto
{
    public Guid CompositeId { get; set; }
    public string? Name { get; set; }
    public double UnitsSold { get; set; }
    public double SaleEarnings { get; set; }
    public double AverageSaleDiscount { get; set; }
    public int ElementCount { get; set; }
    public double MakingCosts { get; set; }
    public double SellingPrice { get; set; }
}
