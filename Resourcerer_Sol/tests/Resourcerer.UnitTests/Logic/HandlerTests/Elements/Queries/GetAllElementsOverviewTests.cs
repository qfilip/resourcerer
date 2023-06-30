using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using MockQueryable.FakeItEasy;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Elements;
using Resourcerer.UnitTests.Utilities.FluentMocks;
using Resourcerer.UnitTests.Utilities.MemoryContext;

namespace Resourcerer.UnitTests.Logic.HandlerTests.Elements.Queries;

public class GetAllElementsOverviewTests
{
    private readonly IAppDbContext _appDbContext;
    private readonly IAppDbContext _testDbContext;

    public GetAllElementsOverviewTests()
    {
        _appDbContext = A.Fake<IAppDbContext>();
        _testDbContext = new ContextCreator().GetTestDbContext();
    }

    [Fact]
    public async Task Foo()
    {
        _testDbContext.Categories.Add(new Category
        {
            Name = "Foo"
        });

        await _testDbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Bar()
    {
        var categories = await _testDbContext.Categories.ToListAsync();
        Assert.True(categories.Count == 7);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_ElementsArePurchased()
    {
        var elementsMock = new List<Element>
        {
            FluentElementMocker
            .Create("5th-element")
            .AddElementPurchasedEvents(new List<(double priceByUnit, int unitsBought)>
            {
                (1d, 5),
                (1d, 5)
            })
            .AddUnitOfMeasure("Foo", "f")
            .Build()
        }.AsQueryable().BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents).Returns(new List<CompositeSoldEvent>().AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsOverview.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, result.Status);

        Assert.Equal(10, result.Object![0].UnitsPurchased);
        Assert.Equal(10d, result.Object![0].PurchaseCosts);
        Assert.Equal(10, result.Object![0].UnitsInStock);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_CompositeIsSold()
    {
        var compositeId = Guid.NewGuid();
        var elementsMock = new List<Element>
        {
            FluentElementMocker
            .Create("5th-element")
            .AddUnitOfMeasure("Foo", "f")
            .AddElementPurchasedEvents(new List<(double priceByUnit, int unitsBought)>
            {
                (1d, 6)
            })
            .AddExcerpts(new List<(Guid compositeId, double quantity)>
            {
                (compositeId, 3)
            })
            .Build()
        }.AsQueryable().BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents)
            .Returns(new List<CompositeSoldEvent> {
                new CompositeSoldEvent
                {
                    CompositeId = compositeId,
                    PriceByUnit = 1,
                    UnitsSold = 1
                },
                new CompositeSoldEvent
                {
                    CompositeId = compositeId,
                    PriceByUnit = 1,
                    UnitsSold = 1
                },
                new CompositeSoldEvent
                {
                    CompositeId = Guid.Empty,
                    PriceByUnit = 1,
                    UnitsSold = 1
                }
            }.AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsOverview.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, result.Status);

        Assert.Equal(6, result.Object![0].UnitsPurchased);
        Assert.Equal(6d, result.Object![0].PurchaseCosts);
        Assert.Equal(0, result.Object![0].UnitsInStock);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_ElementIsSold()
    {
        var elementsMock = new List<Element>
        {
            FluentElementMocker
            .Create("5th-element")
            .AddUnitOfMeasure("Foo", "f")
            .AddElementPurchasedEvents(new List<(double priceByUnit, int unitsBought)>
            {
                (1d, 6)
            })
            .AddElementSoldEvents(new List<(double priceByUnit, int unitsSold)>
            {
                (1d, 3),
                (1d, 3)
            })
            .Build()
        }.AsQueryable().BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents)
            .Returns(new List<CompositeSoldEvent>().AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsOverview.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, result.Status);

        Assert.Equal(6, result.Object![0].UnitsPurchased);
        Assert.Equal(6d, result.Object![0].PurchaseCosts);
        Assert.Equal(0, result.Object![0].UnitsInStock);
    }
}
