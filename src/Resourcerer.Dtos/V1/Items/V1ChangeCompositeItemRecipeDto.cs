namespace Resourcerer.Dtos.V1;

public class V1ChangeCompositeItemRecipe : IDto
{
    public Guid CompositeId { get; set; }
    public Dictionary<Guid, double>? ExcerptMap { get; set; }
}
