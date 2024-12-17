namespace Resourcerer.Dtos.Entity;

public class RecipeDto : EntityDto
{
    public int Version { get; set; }

    // relational
    public Guid CompositeItemId { get; set; }
    public ItemDto? CompositeItem { get; set; }

    public ExcerptDto[] RecipeExcerpts { get; set; } = Array.Empty<ExcerptDto>();
}
