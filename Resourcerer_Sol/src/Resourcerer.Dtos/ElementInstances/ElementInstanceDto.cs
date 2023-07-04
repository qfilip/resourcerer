using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.ElementInstances;

public class ElementInstanceDto : EntityDto
{
    public double Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }

    public Guid ElementId { get; set; }
    public virtual ElementDto? Element { get; set; }
}
