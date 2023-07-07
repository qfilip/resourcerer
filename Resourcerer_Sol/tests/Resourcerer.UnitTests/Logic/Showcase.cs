using FakeItEasy;
using MockQueryable.FakeItEasy;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Queries.Elements;
using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities.FluentMocks;

namespace Resourcerer.UnitTests.Logic;

public class Showcase
{
    private readonly AppDbContext _appDbContext;
    public Showcase()
    {
        _appDbContext = A.Fake<AppDbContext>();
    }

    [Fact(Skip = "Showcasing how to mock DbSet on fake DbContext")]
    public async void CorrectlySumsUsageDetails_When_ElementsArePurchased()
    {
        var elementsMock = new List<Element> {}
            .AsQueryable()
            .BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents).Returns(new List<CompositeSoldEvent>().AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsStatistics.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);

        Assert.Equal(10, result.Object![0].UnitsPurchased);
        Assert.Equal(10d, result.Object![0].PurchaseCosts);
        Assert.Equal(10, result.Object![0].UnitsInStock);
    }

    [Fact(Skip = "Showcasing how to mock DbSet on fake DbContext")]
    public async void CorrectlySumsUsageDetails_When_CompositeIsSold()
    {
        var compositeId = Guid.NewGuid();
        var elementsMock = new List<Element> {}
            .AsQueryable()
            .BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents)
            .Returns(new List<CompositeSoldEvent> {
                new CompositeSoldEvent
                {
                    CompositeId = compositeId,
                    UnitPrice = 1,
                    UnitsSold = 1
                },
                new CompositeSoldEvent
                {
                    CompositeId = compositeId,
                    UnitPrice = 1,
                    UnitsSold = 1
                },
                new CompositeSoldEvent
                {
                    CompositeId = Guid.Empty,
                    UnitPrice = 1,
                    UnitsSold = 1
                }
            }.AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsStatistics.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);

        Assert.Equal(6, result.Object![0].UnitsPurchased);
        Assert.Equal(6d, result.Object![0].PurchaseCosts);
        Assert.Equal(0, result.Object![0].UnitsInStock);
    }

    [Fact(Skip = "Showcasing how to mock DbSet on fake DbContext")]
    public async void CorrectlySumsUsageDetails_When_ElementIsSold()
    {
        var elementsMock = new List<Element> {}
            .AsQueryable()
            .BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        A.CallTo(() => _appDbContext.CompositeSoldEvents)
            .Returns(new List<CompositeSoldEvent>().AsQueryable().BuildMockDbSet());

        var handler = new GetAllElementsStatistics.Handler(_appDbContext);
        var result = await handler.Handle(new Unit());

        Assert.Equal(eHandlerResultStatus.Ok, result.Status);

        Assert.Equal(6, result.Object![0].UnitsPurchased);
        Assert.Equal(6d, result.Object![0].PurchaseCosts);
        Assert.Equal(0, result.Object![0].UnitsInStock);
    }
}
