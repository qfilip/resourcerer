using FakeItEasy;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;
using System.Text.Json;

namespace Resourcerer.UnitTests.Logic;

public class TestsBase
{
    protected readonly TestDbContext _ctx;
    protected readonly JsonSerializerOptions _serializerOptions;
    protected readonly Forger _forger;

    public TestsBase()
    {
        _ctx = new ContextCreator().GetTestDbContext();
        
        _serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        _forger = new Forger(_ctx, DataFaking.FakingFunctions);
    }

    protected ILogger<T> MockLogger<T>() => A.Fake<ILogger<T>>();

    protected static Mapper GetMapster()
    {
        var mapsterConfig = new TypeAdapterConfig();
        void TwoWayMap<TSource, TTarget>()
        {
            mapsterConfig!.NewConfig<TSource, TTarget>().PreserveReference(true);
            mapsterConfig!.NewConfig<TTarget, TSource>().PreserveReference(true);
        }

        TwoWayMap<AppUser, AppUserDto>();
        TwoWayMap<Category, CategoryDto>();
        TwoWayMap<Company, CompanyDto>();
        TwoWayMap<Excerpt, ExcerptDto>();
        TwoWayMap<Instance, InstanceDto>();
        TwoWayMap<Item, ItemDto>();
        TwoWayMap<Price, PriceDto>();
        TwoWayMap<UnitOfMeasure, UnitOfMeasureDto>();

        mapsterConfig.Compile(failFast: true);

        return new(mapsterConfig);
    }
}
