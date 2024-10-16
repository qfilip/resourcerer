namespace Resourcerer.Dtos.V1;

public class V1UpdateCategory : IDto
{
    public Guid CategoryId { get; set; }
    public Guid? NewParentCategoryId { get; set; }
    public string? NewName { get; set; }
}
