namespace Resourcerer.Dtos;
public class CreateItemProductionOrderRequestDto : ItemProductionEventBaseDto
{
    public Guid ItemId { get; set; }
    public double Quantity { get; set; }
}
