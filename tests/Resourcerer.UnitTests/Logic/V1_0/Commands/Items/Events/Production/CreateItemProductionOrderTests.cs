using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
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
            // InstanceToUseIds = fd.Elements.SelectMany(xs => xs.InstanceIds).ToArray()
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        var x = 0;
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
                
                fd.Elements[i].InstanceIds.Add(instance.Id);
            }
        }
           
        DF.FakeExcerpts(ctx, composite, elements.ToArray());

        return fd;
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
    public List<Guid> InstanceIds { get; set; } = new();
}
