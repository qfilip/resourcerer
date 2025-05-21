namespace Resourcerer.Dtos.Entities;
public class InstanceDto : EntityDto
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // relational
    public Guid? ItemId { get; set; }
    public ItemDto? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public CompanyDto? OwnerCompany { get; set; }

    public Guid? SourceInstanceId { get; set; }
    public InstanceDto? SourceInstance { get; set; }

    public InstanceDto[] DerivedInstances { get; set; } = Array.Empty<InstanceDto>();

    public InstanceOrderedEventDto[] OrderedEvents { get; set; } = Array.Empty<InstanceOrderedEventDto>();
    public InstanceReservedEventDto[] ReservedEvents { get; set; } = Array.Empty<InstanceReservedEventDto>();
    public InstanceDiscardedEventDto[] DiscardedEvents { get; set; } = Array.Empty<InstanceDiscardedEventDto>();
}
