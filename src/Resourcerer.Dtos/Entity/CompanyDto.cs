namespace Resourcerer.Dtos.Entity;

public class CompanyDto : EntityDto<CompanyDto>
{
    public string? Name { get; set; }

    public AppUserDto[] Employees { get; set; } = Array.Empty<AppUserDto>();
    public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();
    public InstanceDto[] Instances { get; set; } = Array.Empty<InstanceDto>();
    public UnitOfMeasureDto[] UnitsOfMeasure { get; set; } = Array.Empty<UnitOfMeasureDto>();
}
