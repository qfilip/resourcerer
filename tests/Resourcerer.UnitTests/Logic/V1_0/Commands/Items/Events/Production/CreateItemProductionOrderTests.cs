using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands.Items;
using Resourcerer.UnitTests.Utilities;

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
        var fd = Faking.FakeData(_testDbContext, 2, 2);
        
        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _testDbContext.SaveChanges();
        
        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertCorrectEventsCreated(dto);
    }

    [Fact]
    public void When_ItemNotFound_Then_NotFound()
    {
        // arrange
        Faking.FakeData(_testDbContext, 2, 2);
        var dto = new CreateItemProductionOrderRequestDto { ItemId = Guid.NewGuid() };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_RequestedInstancesNotFound_Then_NotFound()
    {
        // arrange
        var fd = Faking.FakeData(_testDbContext, 2, 2);
        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            InstancesToUse = new Dictionary<Guid, double>
            {
                { Guid.NewGuid(), 2 }
            }
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_NotEnoughInstances_Then_Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_testDbContext, 2, 1);

        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd)
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
        var fd = Faking.FakeData(_testDbContext, 2, 2);

        var dto = new CreateItemProductionOrderRequestDto
        {
            ItemId = fd.CompositeId,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd, () => 0.3)
        };

        _testDbContext.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private void AssertCorrectEventsCreated(CreateItemProductionOrderRequestDto dto)
    {
        var ids = dto.InstancesToUse.Keys.ToArray();
        
        var instances = _testDbContext.Instances
            .Where(x => ids.Contains(x.Id))
            .ToArray();

        var orderEvent = _testDbContext.ItemProductionOrders
            .First(x => x.ItemId == dto.ItemId);

        Assert.True(orderEvent.InstancesUsedIds.All(dto.InstancesToUse.Keys.Contains));

        foreach (var i in instances)
        {
            var qty = dto.InstancesToUse[i.Id];
            i.ReservedEvents.First(ev => ev.Quantity == qty);
        }
    }
}
