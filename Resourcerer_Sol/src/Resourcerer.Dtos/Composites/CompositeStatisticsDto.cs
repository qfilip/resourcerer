using FluentValidation;

namespace Resourcerer.Dtos;

public class CompositeStatisticsDto : IBaseDto<CompositeStatisticsDto>
{
    public Guid CompositeId { get; set; }
    public string? Name { get; set; }
    public double UnitsSold { get; set; }
    public double SaleEarnings { get; set; }
    public double AverageSaleDiscount { get; set; }
    public int ElementCount { get; set; }
    public double MakingCosts { get; set; }
    public double SellingPrice { get; set; }

    public AbstractValidator<CompositeStatisticsDto>? GetValidator() => null;
}
