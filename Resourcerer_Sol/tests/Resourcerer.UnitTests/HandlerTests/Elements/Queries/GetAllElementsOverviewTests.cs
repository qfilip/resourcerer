using FakeItEasy;
using MockQueryable.FakeItEasy;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic;
using Resourcerer.Logic.Elements.Queries;

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
        var elementsMock = new List<Element>().AsQueryable().BuildMockDbSet();
        A.CallTo(() => _appDbContext.Elements).Returns(elementsMock);
        var handler = new GetAllElementsOverview.Handler(_appDbContext);

        await handler.Handle(new Unit());
    }
}
