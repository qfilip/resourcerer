using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Items.Events.Production;

internal class Faking
{
    internal static FakedData FakeData(AppDbContext ctx, int elementCount, int instanceCount)
    {
        var composite = DF.FakeItem(ctx);
        var fd = new FakedData()
        {
            CompositeId = composite.Id
        };

        var elements = new List<(Item, double)>();

        for (int i = 0; i < elementCount; i++)
            elements.Add((DF.FakeItem(ctx), 1));

        for (int i = 0; i < elements.Count; i++)
        {
            fd.Elements.Add(new FakedItem { Item = elements[i].Item1 });

            for (int j = 0; j < instanceCount; j++)
            {
                var instance = DF.FakeInstance(ctx, x =>
                {
                    x.ItemId = elements[i].Item1.Id;
                    x.Item = elements[i].Item1;
                    x.Quantity = 1;
                });

                fd.Elements[i].Instances.Add(instance);
            }
        }

        DF.FakeExcerpts(ctx, composite, elements.ToArray());

        return fd;
    }
    internal static ItemProductionOrder FakeOrder(TestDbContext context, FakedData data, Action<ItemProductionOrder>? modifier = null)
    {
        var order = DF.FakeItemProductionOrder(context, x =>
        {
            x.ItemId = data.CompositeId;
            x.InstancesUsedIds = MapInstancesToUse(data).Keys.ToArray();
            x.Reason = "test";
        });
        
        modifier?.Invoke(order);

        return order;
    }
    internal static Dictionary<Guid, double> MapInstancesToUse(FakedData fd, Func<double>? valueModifier = null)
    {
        var dict = new Dictionary<Guid, double>();

        fd.Elements.ForEach(x =>
            x.Instances.ForEach(i =>
            {
                var val = valueModifier?.Invoke() ?? i.Quantity;
                dict.Add(i.Id, val);
            }));

        return dict;
    }
}

internal class FakedData
{
    public Guid CompositeId { get; set; }
    public List<FakedItem> Elements { get; set; } = new();
}

internal class FakedItem
{
    public Item Item { get; set; } = new();
    public List<Instance> Instances { get; set; } = new();
}