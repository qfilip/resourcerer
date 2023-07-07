using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Composites;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.TestDatabaseMocks;
using Resourcerer.Utilities;
using EvMock = Resourcerer.UnitTests.Utilities.TestDatabaseMocks.CarpenterDbEventMocker;

namespace Resourcerer.UnitTests.Logic.Queries.Composites;

public class GetAllCompositesStatisticsTests
{
    private readonly AppDbContext _testDbContext;
    private readonly GetAllCompositesStatistics.Handler _handler;
    public GetAllCompositesStatisticsTests()
    {
        _testDbContext = new ContextCreator(CarpenterDbMocker.GetSeed).GetTestDbContext();
        _handler = new GetAllCompositesStatistics.Handler(_testDbContext);
    }

    [Fact]
    public async Task CorrectlySums_MakingCosts_And_ElementCount()
    {
        var (window, boat) = EvMock.GetWindowAndBoat(_testDbContext);
        
        var hResult = await _handler.Handle(new Unit());
        Assert.Equal(eHandlerResultStatus.Ok, hResult.Status);

        var windowStats = hResult.Object!.First(x => x.Name == "window");
        var boatStats = hResult.Object!.First(x => x.Name == "boat");

        var windowExpected = GetExpected(window, 5, 1, new List<CompositeSoldEvent>());
        var boatExpected = GetExpected(boat, 125, 2, new List<CompositeSoldEvent>());
        
        Assert.Equivalent(windowExpected, windowStats);
        Assert.Equivalent(boatExpected, boatStats);
    }

    private CompositeStatisticsDto GetExpected(Composite c, double makingCosts, int elementCount, List<CompositeSoldEvent> evs)
    {
        return new CompositeStatisticsDto
        {
            CompositeId = c.Id,
            Name = c.Name,
            SellingPrice = c.Prices.Single().UnitValue,
            UnitsSold = evs.Sum(x => x.UnitsSold),
            AverageSaleDiscount = Maths.SafeAverage(evs, x => x.TotalDiscountPercent),
            SaleEarnings = evs.Sum(x => Maths.Discount(x.UnitsSold * x.UnitPrice, x.TotalDiscountPercent)),
            MakingCosts = makingCosts,
            ElementCount = elementCount
        };
    }
}
