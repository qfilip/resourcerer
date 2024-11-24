using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Fake;

public class DataSeedDto : IDto
{
    public AppUser[] AppUsers { get; set; } = Array.Empty<AppUser>();
    public Company[] Companies { get; set; } = Array.Empty<Company>();
    public Category[] Categories { get; set; } = Array.Empty<Category>();
    public Recipe[] Recipes { get; set; } = Array.Empty<Recipe>();
    public RecipeExcerpt[] Excerpts { get; set; } = Array.Empty<RecipeExcerpt>();
    public UnitOfMeasure[] UnitsOfMeasure { get; set; } = Array.Empty<UnitOfMeasure>();
    public Price[] Prices { get; set; } = Array.Empty<Price>();
    public Item[] Items { get; set; } = Array.Empty<Item>();
    public ItemProductionOrder[] ItemProductionOrders { get; set; } = Array.Empty<ItemProductionOrder>();
    public Instance[] Instances { get; set; } = Array.Empty<Instance>();
    public InstanceOrderedEvent[] InstanceOrderedEvents { get; set; } = Array.Empty<InstanceOrderedEvent>();
    public InstanceReservedEvent[] InstanceReservedEvents { get; set; } = Array.Empty<InstanceReservedEvent>();
    public InstanceDiscardedEvent[] InstanceDiscardedEvents { get; set; } = Array.Empty<InstanceDiscardedEvent>();
}
