﻿using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items.Events.Production;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Resourcerer.UnitTests.Logic.V1.Items.Events.Production;

public class CreateCompositeItemProductionOrderTests : TestsBase
{
    private readonly CreateCompositeItemProductionOrder.Handler _sut;
    public CreateCompositeItemProductionOrderTests()
    {
        _sut = new CreateCompositeItemProductionOrder.Handler(_ctx);
    }

    [Fact]
    public void HappyPath()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.Composite!.Category!.Company!.Id,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertCorrectEventsCreated(dto);
    }

    [Fact]
    public void ItemNotComposite__Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);
        var item = _forger.Fake<Item>();

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = item.Id,
            CompanyId = item.Category!.Company!.Id,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InstantProduction_CreatesInstanceAndEvents()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.Composite!.Category!.Company!.Id,
            Quantity = 2,
            InstantProduction = true,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        AssertCorrectEventsCreated(dto, true);
    }

    [Fact]
    public void When_ItemNotFound_Then_NotFound()
    {
        // arrange
        Faking.FakeData(_forger, 2, 2);
        var dto = new V1CreateCompositeItemProductionOrderCommand { ItemId = Guid.NewGuid() };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_RequestedInstancesNotFound_Then_NotFound()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);
        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.CompanyId,
            InstancesToUse = new Dictionary<Guid, double>
            {
                { Guid.NewGuid(), 2 }
            }
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_RequestedInstancesDontBelongToCompany_Then_Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);
        var otherCompany = _forger.Fake<Company>();

        fd.Elements.ForEach(e => e.Instances.ForEach(i =>
        {
            i.OwnerCompany = otherCompany;
        }));

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.CompanyId,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_NotEnoughInstances_Then_Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 1);

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.CompanyId,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_IncorrectInstanceQuantitySpecified_Then_Rejected()
    {
        // arrange
        var fd = Faking.FakeData(_forger, 2, 2);

        var dto = new V1CreateCompositeItemProductionOrderCommand
        {
            ItemId = fd.Composite!.Id,
            CompanyId = fd.CompanyId,
            Quantity = 2,
            InstancesToUse = Faking.MapInstancesToUse(fd, () => 0.3)
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private void AssertCorrectEventsCreated(V1CreateCompositeItemProductionOrderCommand dto, bool instantProductionChecks = false)
    {
        var instanceIds = dto.InstancesToUse.Keys.ToArray();

        var instances = _ctx.Instances
            .Where(x => instanceIds.Contains(x.Id))
            .ToArray();

        var composite = _ctx.Items
            .Include(x => x.Recipes)
            .First(x => x.Id == dto.ItemId);

        var recipe = composite.Recipes
            .OrderByDescending(x => x.Version)
            .First();

        var orderEvent = _ctx.ItemProductionOrders
            .First(x => x.ItemId == dto.ItemId);
        
        var newInstance = _ctx.Instances
            .Single(x =>
                x.OwnerCompanyId == orderEvent.CompanyId &&
                x.ItemId == dto.ItemId &&
                x.Quantity == dto.Quantity
            );

        Assert.True(orderEvent.InstancesUsedIds.All(dto.InstancesToUse.Keys.Contains));
        Assert.Equal(recipe.Version, orderEvent.ItemRecipeVersion);
        
        foreach (var i in instances)
        {
            var qty = dto.InstancesToUse[i.Id];
            i.ReservedEvents.First(ev => ev.Quantity == qty);
        }

        if (instantProductionChecks)
        {
            Assert.NotNull(orderEvent.StartedEvent);
            Assert.NotNull(orderEvent.FinishedEvent);
            
        }
    }
}
