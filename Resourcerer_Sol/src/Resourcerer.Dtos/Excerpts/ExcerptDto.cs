using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.Excerpts;

public class ExcerptDto : EntityDto
{
    public Guid CompositeId { get; set; }
    public Guid ElementId { get; set; }

    public CompositeDto? Composite { get; set; }
    public ElementDto? Element { get; set; }

    public double Quantity { get; set; }
}
