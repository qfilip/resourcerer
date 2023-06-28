using FakeItEasy;
using MockQueryable.FakeItEasy;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic;
using Resourcerer.Logic.Elements.Queries;
using Resourcerer.UnitTests.Utilities.FluentMocks;

namespace Resourcerer.UnitTests.HandlerTests.Elements.Queries;

public class GetAllElementsOverviewTests
{
    private readonly IAppDbContext _appDbContext;
    
    public GetAllElementsOverviewTests()
    {
        _appDbContext = A.Fake<IAppDbContext>();
    }

    [Fact]
    public async void Foo()
    {
        var composite1Id = Guid.NewGuid();
        var element = FluentElementMocker
                .WithName("5th-element")
                .AddElementPurchasedEvents(new List<(double priceByUnit, int unitsBought)>
                {
                    (1d, 5),
                    (1d, 5)
                })
                .AddElementSoldEvents(new List<(double priceByUnit, int unitsSold)>
                {
                    (2d, 1),
                    (2d, 1)
                })
                .AddExcerpts(new List<(Guid compositeId, double quantity)>
                {
                    (composite1Id, 1)
                })
                .AddUnitOfMeasure("foo", "f")
                .Build();

        var elementsMock = new List<Element> { element }.AsQueryable().BuildMockDbSet();

        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);

        var compositeSoldEventMocks = new List<CompositeSoldEvent>
        {
            new CompositeSoldEvent
            {
                CompositeId = composite1Id,
                PriceByUnit = 1,
                UnitsSold = 1
            }
        }.AsQueryable().BuildMockDbSet();

        A.CallTo(() => _appDbContext.CompositeSoldEvents).Returns(compositeSoldEventMocks);
        
        
        var handler = new GetAllElementsOverview.Handler(_appDbContext);

        var result = await handler.Handle(new Unit());

        if(result.Status == HandlerResultStatus.Ok)
        {

        }
    }
}
