namespace Resourcerer.Dtos.V1;

public class V1EditUnitOfMeasure : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
}
