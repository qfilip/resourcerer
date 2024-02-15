namespace Resourcerer.Dtos;

public class CreateCategoryDto : IBaseDto
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Guid CompanyId { get; set; }
}

