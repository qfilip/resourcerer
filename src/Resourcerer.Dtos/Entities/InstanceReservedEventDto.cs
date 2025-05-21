using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.Entities.Json;

namespace Resourcerer.Dtos;

public class InstanceReservedEventDto : EntityDto
{
    public Guid ItemProductionOrderId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public InstanceDto? Instance { get; set; }

    // json
    public InstanceReserveCancelledEventDto? CancelledEvent { get; set; }
    public InstanceReserveUsedEventDto? UsedEvent { get; set; }
}
