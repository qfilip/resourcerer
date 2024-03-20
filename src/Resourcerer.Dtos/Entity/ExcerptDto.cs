namespace Resourcerer.Dtos.Entity;

public class ExcerptDto : IDto
{
    public Guid CompositeId { get; set; }
    public Guid ElementId { get; set; }

    public ItemDto? Element { get; set; }

    public double Quantity { get; set; }
}
