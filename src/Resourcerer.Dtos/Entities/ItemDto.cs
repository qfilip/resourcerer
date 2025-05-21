namespace Resourcerer.Dtos.Entities;

public class ItemDto : EntityDto
{
    public string? Name { get; set; }
    public decimal ProductionPrice { get; set; }
    public double ProductionTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }

    // relational

    public Guid CategoryId { get; set; }
    public virtual CategoryDto? Category { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public virtual UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public ExcerptDto[] ElementExcerpts { get; set; } = Array.Empty<ExcerptDto>();
    public ExcerptDto[] CompositeExcerpts { get; set; } = Array.Empty<ExcerptDto>();
    public PriceDto[] Prices { get; set; } = Array.Empty<PriceDto>();
    public InstanceDto[] Instances { get; set; } = Array.Empty<InstanceDto>();
    public ItemProductionOrderDto[] ProductionOrders { get; set; } = Array.Empty<ItemProductionOrderDto>();
}


