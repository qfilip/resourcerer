﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

internal class Faking
{
    internal static FakedData FakeData(TestDbContext ctx, int elementCount, int instanceCount)
    {
        var company = DF.Fake<Company>(ctx);
        var composite = DF.Fake<Item>(ctx, x =>
        {
            x.Category = DF.Fake<Category>(ctx, x => x.Company = company);
        });
        var fd = new FakedData()
        {
            Composite = composite,
            CompanyId = company.Id
        };

        var elements = new List<(Item, double)>();

        for (int i = 0; i < elementCount; i++)
            elements.Add((DF.Fake<Item>(ctx, x =>
            {
                x.Category = DF.Fake<Category>(ctx, x => x.Company = company);
            }), 1));

        for (int i = 0; i < elements.Count; i++)
        {
            fd.Elements.Add(new FakedItem { Item = elements[i].Item1 });

            for (int j = 0; j < instanceCount; j++)
            {
                var instance = DF.Fake<Instance>(ctx, x =>
                {
                    x.Quantity = 1;

                    x.Item = elements[i].Item1;
                    x.OwnerCompany = company;
                });
                fd.Elements[i].Instances.Add(instance);
            }
        }

        foreach (var element in elements)
        {
            DF.Fake<Excerpt>(ctx, x =>
            {
                x.Composite = composite;
                x.Element = element.Item1;

                x.Quantity = element.Item2;
            });
        }

        return fd;
    }
    internal static ItemProductionOrder FakeOrder(TestDbContext context, FakedData data, Action<ItemProductionOrder>? modifier = null)
    {
        return DF.Fake<ItemProductionOrder>(context, x =>
        {
            x.Item = data.Composite;
            x.CompanyId = data.CompanyId;
            x.InstancesUsedIds = MapInstancesToUse(data).Keys.ToArray();
            x.Reason = "test";

            modifier?.Invoke(x);
        });
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
    public Item? Composite { get; set; }
    public Guid CompanyId { get; set; }
    public List<FakedItem> Elements { get; set; } = new();
}

internal class FakedItem
{
    public Item Item { get; set; } = new();
    public List<Instance> Instances { get; set; } = new();
}