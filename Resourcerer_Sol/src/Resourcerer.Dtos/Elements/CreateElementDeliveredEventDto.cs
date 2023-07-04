namespace Resourcerer.Dtos;

public class CreateElementDeliveredEventDto : BaseDto
{
    public Guid ElementPurchasedEventId { get; set; }
    public DateTime? InstanceExpiryDate { get; set; }
}
