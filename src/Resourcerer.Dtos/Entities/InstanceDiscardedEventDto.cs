namespace Resourcerer.Dtos.Entities;

public class InstanceDiscardedEventDto : EntityDto
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public InstanceDto? Instance { get; set; }
}
