namespace Resourcerer.Dtos.V1;

public class V1ChangePrice : IDto
{
    public Guid ItemId { get; set; }
    public double UnitPrice { get; set; }
}


