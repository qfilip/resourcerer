using Resourcerer.UnitTests.Utilities.TestDatabaseMocks;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Logic.Commands.ElementEvents;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

public class CreateElementDeliveredEventTests
{
    private readonly IAppDbContext _testDbContext;
    private readonly CreateElementDeliveredEvent.Handler _handler;
    public CreateElementDeliveredEventTests()
    {
        _testDbContext = new ContextCreator(CarpenterDbMocker.GetSeed).GetTestDbContext();
        _handler = new CreateElementDeliveredEvent.Handler(_testDbContext);
    }

    [Fact]
    public async Task Ok_When_ElementPurchasedEvent_Exists() { }

    [Fact]
    public async Task ValidationError_When_ElementPurchaseCancelledEvent_Exists() { }

    [Fact]
    public async Task IsIdempotent() { }

    [Fact]
    public async Task ThrowsException_When_ElementPurchasedEvent_DoesntExist() { }
}
