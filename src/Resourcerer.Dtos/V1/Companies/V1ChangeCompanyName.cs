namespace Resourcerer.Dtos.V1;

public class V1ChangeCompanyName : IDto
{
    public Guid CompanyId { get; set; }
    public string? NewName { get; set; }
}
