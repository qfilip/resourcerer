namespace Resourcerer.Dtos.V1;

public class V1ChangeItemCategory : IDto
{
    public Guid ItemId { get; set; }
    public Guid NewCategoryId { get; set; }
}
