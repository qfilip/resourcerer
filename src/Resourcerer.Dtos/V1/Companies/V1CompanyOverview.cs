using Resourcerer.Dtos.Entities;

namespace Resourcerer.Dtos.V1;

public class V1CompanyOverview : IDto
{
    public string? Name { get; set; }
    public AppUserDto[] Employees { get; set; } = Array.Empty<AppUserDto>();
    public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();
}
