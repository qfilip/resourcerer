namespace Resourcerer.Dtos.Entities.Json;

public class InstanceOrderCancelledEventDto : EntityDto
{
    public string? Reason { get; set; }
    public decimal RefundedAmount { get; set; }
}
