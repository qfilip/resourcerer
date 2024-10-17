namespace Resourcerer.Dtos.V1;

public class V1ItemShoppingDetails : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? CategoryName { get; set; }
    public Guid CompanyId { get; set; }
    public string? CompanyName { get; set; }
}
