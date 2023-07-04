namespace Resourcerer.Dtos;

public class ElementInstanceDto : EntityDto
{
    public double Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }

    public Guid ElementId { get; set; }
    public virtual ElementDto? Element { get; set; }
}
