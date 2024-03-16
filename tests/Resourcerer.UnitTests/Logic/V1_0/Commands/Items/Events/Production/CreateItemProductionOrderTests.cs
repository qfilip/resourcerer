using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands.Items;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Items.Events.Production;

public class CreateItemProductionOrderTests : TestsBase
{
    private readonly CreateItemProductionOrder.Handler _sut;
    public CreateItemProductionOrderTests()
    {
        _sut = new CreateItemProductionOrder.Handler(_testDbContext);
    }

    [Fact]
    public void HappyPath()
    {
        // arrange
        var fd = FakeData(_testDbContext, 2, 2);
        
        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = MapInstancesToUse(fd)
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertCorrectEventsCreated(dto.InstancesToUse);
    }

    [Fact]
    public void When_NotEnoughInstances_Then_Rejected()
    {
        // arrange
        var fd = FakeData(_testDbContext, 2, 1);

        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = MapInstancesToUse(fd)
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_IncorrectInstanceQuantitySpecified_Then_Rejected()
    {
        // arrange
        var fd = FakeData(_testDbContext, 2, 2);

        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = MapInstancesToUse(fd, () => 0.3)
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private void AssertCorrectEventsCreated(Dictionary<Guid, double> instancesToUse)
    {
        var ids = instancesToUse.Keys.ToArray();
        
        var instances = _testDbContext.Instances
            .Where(x => ids.Contains(x.Id))
            .ToArray();

        foreach (var i in instances)
        {
            var qty = instancesToUse[i.Id];
            i.ReservedEvents.First(ev => ev.Quantity == qty);
        }
    }

    private static FakedData FakeData(AppDbContext ctx, int elementCount, int instanceCount)
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
            fd.Elements.Add(new FakedItem { ItemId = elements[i].Item1.Id });

            for(int j = 0; j < instanceCount; j++)
            {
                var instance = DF.FakeInstance(ctx, x =>
                {
                    x.ItemId = elements[i].Item1.Id;
                    x.Quantity = 1;
                });
                
                fd.Elements[i].Instances.Add(instance);
            }
        }
           
        DF.FakeExcerpts(ctx, composite, elements.ToArray());

        return fd;
    }

    private static Dictionary<Guid, double> MapInstancesToUse(FakedData fd, Func<double>? valueModifier = null)
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
    public Guid ItemId { get; set; }
    public List<Instance> Instances { get; set; } = new();
}
