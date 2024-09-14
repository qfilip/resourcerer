using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

public class CreateElementItemProductionOrderTests : TestsBase
{
    private readonly CreateElementItemProductionOrder.Handler _sut;
    public CreateElementItemProductionOrderTests()
    {
        _sut = new CreateElementItemProductionOrder.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var command = new V1CreateElementItemProductionOrderCommand
        {
            ItemId = item.Id,
            CompanyId = item.Category!.CompanyId,
            Quantity = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);

        var order = _ctx.ItemProductionOrders
            .Single(x =>
                x.ItemId == command.ItemId &&
                x.CompanyId == command.CompanyId);

        Assert.Multiple(
            () => Assert.NotNull(order),
            () => Assert.Equal(command.Quantity, order.Quantity)
        );
    }

    [Fact]
    public void InstantProduction__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var command = new V1CreateElementItemProductionOrderCommand
        {
            ItemId = item.Id,
            CompanyId = item.Category!.CompanyId,
            Quantity = 2,
            InstantProduction = true
        };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);

        var order = _ctx.ItemProductionOrders
            .Single(x =>
                x.ItemId == command.ItemId &&
                x.CompanyId == command.CompanyId);

        var newInstance = _ctx.Instances
            .First(x =>
                x.ItemId == command.ItemId &&
                x.OwnerCompanyId == command.CompanyId);

        Assert.Multiple(
            () => Assert.Equal(command.Quantity, order.Quantity),
            () => Assert.Equal(command.Quantity, newInstance.Quantity),
            () => Assert.NotNull(order.StartedEvent),
            () => Assert.NotNull(order.FinishedEvent));
    }

    [Fact]
    public void ItemNotFound__NotFound()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var command = new V1CreateElementItemProductionOrderCommand
        {
            ItemId = Guid.NewGuid(),
            CompanyId = item.Category!.CompanyId,
            Quantity = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(command).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
