namespace Resourcerer.Dtos.V1;

public class V1ChangeItemName : IDto
{
    public Guid ItemId { get; set; }
    public string? NewName { get; set; }
}
