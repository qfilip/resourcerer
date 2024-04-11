using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Excerpt Map(ExcerptDto src) =>
        Map(() =>
            new Excerpt
            {
                Quantity = src.Quantity,

                CompositeId = src.CompositeId,
                Composite = Map(src.Composite, Map),

                ElementId = src.ElementId,
                Element = Map(src.Element, Map)
            }, src);

    public static ExcerptDto Map(Excerpt src) =>
        Map(() =>
            new ExcerptDto
            {
                Quantity = src.Quantity,

                CompositeId = src.CompositeId,
                Composite = Map(src.Composite, Map),

                ElementId = src.ElementId,
                Element = Map(src.Element, Map)
            }, src);
}
