namespace Resourcerer.Dtos;

public class ExcerptDto : EntityDto
{
    public Guid CompositeId { get; set; }
    public Guid ElementId { get; set; }

    public CompositeDto? Composite { get; set; }
    public ElementDto? Element { get; set; }

    public double Quantity { get; set; }
}
