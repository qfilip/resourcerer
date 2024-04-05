namespace Resourcerer.Dtos.V1;

public class V1Register : IDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? CompanyName { get; set; }
}
