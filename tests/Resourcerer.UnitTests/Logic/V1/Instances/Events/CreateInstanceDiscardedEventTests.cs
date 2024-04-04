using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.V1.Instances.Events;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Instances.Events;

public class CreateInstanceDiscardedEventTests : TestsBase
{
    public readonly CreateInstanceDiscardedEvent.Handler _handler;
    public CreateInstanceDiscardedEventTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x => x.Instance = sourceInstance);

        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = sourceInstance.Id,
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

                var instance = _ctx.Instances
                    .Include(x => x.DiscardedEvents)
                    .First(x => x.Id == orderEvent.Instance!.Id);

                Assert.True(instance.DiscardedEvents.Any());
            }
        );
    }

    [Fact]
    public void OrderNotFound__NotFound()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x => x.Instance = sourceInstance);

        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = Guid.NewGuid()
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void QuantityLeft_SmallerThan_Zero__Exception()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx, x => x.Quantity = 1);
        DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
        });
        DF.Fake<InstanceReservedEvent>(_ctx, x => x.Instance = sourceInstance);
        DF.Fake<InstanceDiscardedEvent>(_ctx, x => x.Instance = sourceInstance);

        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = sourceInstance.Id
        };

        _ctx.SaveChanges();

        // act
        var act = () => _handler.Handle(dto).Await();

        // assert
        Assert.Throws<DataCorruptionException>(act);
    }

    [Fact]
    public void QuantityLeft_Equals_Zero__Rejected()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx, x => x.Quantity = 1);
        DF.Fake<InstanceDiscardedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.Quantity = sourceInstance.Quantity;
        });

        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = sourceInstance.Id
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void RequestedDiscardQuantity_LargerThan_QuantityLeft__Rejected()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx, x => x.Quantity = 1);
        DF.Fake<InstanceDiscardedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.Quantity = sourceInstance.Quantity;
        });

        var dto = new V1InstanceDiscardedRequest
        {
            InstanceId = sourceInstance.Id,
            Quantity = 2
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
