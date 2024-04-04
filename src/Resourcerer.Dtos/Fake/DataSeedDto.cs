using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Fake;

public class DataSeedDto : IDto
{
    public List<AppUser> AppUsers { get; set; } = new();
    public List<Company> Companies { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Excerpt> Excerpts { get; set; } = new();
    public List<UnitOfMeasure> UnitsOfMeasure { get; set; } = new();
    public List<Price> Prices { get; set; } = new();
    // items = new();
    public List<Item> Items { get; set; } = new();
    public List<ItemProductionOrder> ItemProductionOrders { get; set; } = new();
    // instances = new();
    public List<Instance> Instances { get; set; } = new();
    public List<InstanceOrderedEvent> InstanceOrderedEvents { get; set; } = new();
    public List<InstanceReservedEvent> InstanceReservedEvents { get; set; } = new();
    public List<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; } = new();
}
