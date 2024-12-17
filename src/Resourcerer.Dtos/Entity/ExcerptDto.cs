namespace Resourcerer.Dtos.Entity;

public class ExcerptDto : EntityDto
{
    public double Quantity { get; set; }
    
    public Guid CompositeId { get; set; }
    public ItemDto? Composite{ get; set; }

    public Guid ElementId { get; set; }
    public ItemDto? Element { get; set; }
}
