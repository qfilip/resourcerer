namespace Resourcerer.Dtos.Entity;
public class InstanceDto : IDto
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public ItemDto? Element { get; set; }

}
