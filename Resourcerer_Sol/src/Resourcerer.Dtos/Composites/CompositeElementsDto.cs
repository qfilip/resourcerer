using FluentValidation;

namespace Resourcerer.Dtos;

public class CompositeElementsDto : IBaseDto<CompositeElementsDto>
{
    public Guid ElementId { get; set; }
    public double Quantity { get; set; }

    public AbstractValidator<CompositeElementsDto>? GetValidator() => null;
}
