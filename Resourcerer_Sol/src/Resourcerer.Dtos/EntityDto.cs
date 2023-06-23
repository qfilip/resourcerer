namespace Resourcerer.Dtos;

public class EntityDto : BaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
