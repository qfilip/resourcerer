using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.HandlerTests.Elements.Queries;

public class GetAllElementsOverviewTests
{
    private readonly IAppDbContext _testDbContext;

    public GetAllElementsOverviewTests()
    {
        
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

    
}
