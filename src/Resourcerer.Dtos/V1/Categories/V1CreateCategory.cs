namespace Resourcerer.Dtos.V1;

public class V1CreateCategory : IDto
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Guid CompanyId { get; set; }
}

