namespace Resourcerer.Dtos;

public class CreateUnitOfMeasureDto : IBaseDto
{
    public Guid CompanyId { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
}
