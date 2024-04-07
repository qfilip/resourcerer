namespace Resourcerer.Dtos.Entity;
public class InstanceDto : EntityDto<InstanceDto>
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // relational
    public Guid? ItemId { get; set; }
    public ItemDto? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public CompanyDto? OwnerCompany { get; set; }

    public Guid? SourceInstanceId { get; set; }
    public virtual InstanceDto? SourceInstance { get; set; }

    public InstanceDto[] DerivedInstances { get; set; } = Array.Empty<InstanceDto>();

}
