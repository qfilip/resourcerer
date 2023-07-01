using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Mocks;
public class DatabaseData
{
    public IEnumerable<AppUser>? AppUsers { get; set; }
    public IEnumerable<Category>? Categories { get; set; }
    public IEnumerable<Excerpt>? Excerpts { get; set; }
    public IEnumerable<UnitOfMeasure>? UnitOfMeasures { get; set; }
    public IEnumerable<OldPrice>? OldPrices { get; set; }

    public IEnumerable<Composite>? Composites { get; set; }
    public IEnumerable<CompositeSoldEvent>? CompositeSoldEvents { get; set; }
    
    public IEnumerable<Element>? Elements { get; set; }
    public IEnumerable<ElementSoldEvent>? ElementSoldEvents { get; set; }
    public IEnumerable<ElementPurchasedEvent>? ElementPurchasedEvents { get; set; }
}
