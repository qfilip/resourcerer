namespace Resourcerer.Dtos.V1;

public class V1CreateUnitOfMeasure : IDto
{
    public Guid CompanyId { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
}
