﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;
using Resourcerer.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Instances;

public class CreateInstanceOrderCancelledEventTests : TestsBase
{
    private readonly CreateInstanceOrderCancelledEvent.Handler _handler;
    public CreateInstanceOrderCancelledEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var instance = _testDbContext.Instances.First(x => x.Id == sourceInstance.Id);
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.NotNull(instance.OrderedEvents[0].OrderCancelledEvent);
    }

    [Fact]
    public void When_OrderEvent_NotFound_Then_Rejected()
    {
        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = MiniId.Generate()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DeliveredEvent_Exists_Then_Rejected()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
        });

        _testDbContext.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_SentEvent_Exists_Then_Rejected()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.SentEvent = DF.FakeSentEvent();
        });

        _testDbContext.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
