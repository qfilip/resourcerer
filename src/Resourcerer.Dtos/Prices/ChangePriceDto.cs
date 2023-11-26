using FluentValidation;

namespace Resourcerer.Dtos;

public class ChangePriceDto : IBaseDto
{
    public Guid ItemId { get; set; }
    public double UnitPrice { get; set; }
}


