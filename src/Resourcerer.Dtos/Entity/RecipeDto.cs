using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Entity;

public class RecipeDto : EntityDto<ExcerptDto>
{
    public int Version { get; set; }

    // relational
    public Guid CompositeItemId { get; set; }
    public Item? CompositeItem { get; set; }

    public ExcerptDto[] RecipeExcerpts { get; set; } = Array.Empty<ExcerptDto>();
}
