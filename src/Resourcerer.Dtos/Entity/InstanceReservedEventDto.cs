using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.Entity;

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
    public InstanceReserveCancelledEvent? CancelledEvent { get; set; }
    public InstanceReserveUsedEvent? UsedEvent { get; set; }
}
