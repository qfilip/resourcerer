using FakeItEasy;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Logic.Fake;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;
    protected readonly JsonSerializerOptions _serializerOptions;
    protected readonly Forger _forger;
    protected readonly IMapper _mapper;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
        
        _serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        _forger = new Forger(_ctx, DF.FakingFunctions);
        _mapper = GetMapster();
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    protected static Mapper GetMapster()
    {
        var mapsterConfig = Resourcerer.Api.Services.ServiceRegistry.GetMapsterConfig();
        return new Mapper(mapsterConfig);
    }

    [Fact]
    public void SeedTest()
    {
        var f = _forger;
        f.Fake<Company>(co =>
        {
            co.Name = "Charm Farm";
            var emps = new List<AppUser>()
            {
                f.Fake<AppUser>(u => u.Name = "Liam"),
                f.Fake<AppUser>(u => u.Name = "Sophia")
            };

            emps.ForEach(e => e.CompanyId = co.Id);

            co.Categories = [
                f.Fake<Category>(ctg => {
                    ctg.CompanyId = co.Id;
                    ctg.Name = "Products";
                    ctg.ChildCategories = [
                        f.Fake<Category>(sc => sc.Name = "Meat"),
                        f.Fake<Category>(sc => sc.Name = "Veggies"),
                        f.Fake<Category>(sc => sc.Name = "Fruit")
                    ];
                })
            ];
        });

        _ctx.SaveChanges();

        var c = _ctx.Companies
            .Include(x => x.Categories)
                .ThenInclude(x => x.ChildCategories)
            .Include(x => x.Employees)
            .ToArray();

        var x = 0;
    }
}
