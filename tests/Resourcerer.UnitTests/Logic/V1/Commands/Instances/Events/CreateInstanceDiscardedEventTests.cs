using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Instances;

public class CreateInstanceDiscardedEventTests : TestsBase
{
    public readonly CreateInstanceDiscardedEvent.Handler _handler;
    public CreateInstanceDiscardedEventTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var derivedInstance = DF.Fake<Instance>(_ctx, x => x.SourceInstance = sourceInstance);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.DerivedInstanceId = derivedInstance.Id;
        });
        
        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = derivedInstance.Id,
            Quantity = orderEvent.Quantity
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var instance = _ctx.Instances.First(x => x.Id == orderEvent.DerivedInstanceId);
                Assert.True(instance.DiscardedEvents.Any());
            }
        );
    }

    [Fact]
    public void When_NotOrdered_Then_NotFound()
    {
        // arrange
        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_QuantityLeft_SmallerThan_Zero_Then_Exception()
    {
        Assert.Fail("Not implemented");
    }

    [Fact]
    public void When_QuantityLeft_Equals_Zero_Then_Rejected()
    {

    }

    [Fact]
    public void When_RequestedDiscardQuantity_LargerThan_QuantityLeft_Then_Rejected()
    {

    }
}
