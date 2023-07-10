//using Resourcerer.DataAccess.Contexts;
//using Resourcerer.DataAccess.Entities;
//using Resourcerer.Dtos;
//using Resourcerer.Logic;
//using Resourcerer.Logic.Commands.ElementEvents;
//using Resourcerer.UnitTests.Utilities;
//using Resourcerer.UnitTests.Utilities.Mocker;

//namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

//public class CreateElementPurchaseCancelledEventTests
//{
//    private readonly AppDbContext _testDbContext;
//    private readonly ElementPurchasedEvent _testPurchasedEvent;
//    private readonly CreateElementPurchaseCancelledEvent.Handler _handler;

//    public CreateElementPurchaseCancelledEventTests()
//    {
//        _testDbContext = new ContextCreator().GetTestDbContext();
//        _testPurchasedEvent = Mocker.MockElementPurchasedEvent(_testDbContext);
//        _handler = new CreateElementPurchaseCancelledEvent.Handler(_testDbContext);
//    }

//    [Fact]
//    public void When_ElementPurchasedEvent_Exists_Then_Ok()
//    {
//        // arrange
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.SaveChanges();

//        var dto = new ElementPurchaseCancelledEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id,
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();
//        var entities = _testDbContext.ElementPurchaseCancelledEvents.ToList();

//        // assert
//        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
//        Assert.Single(entities);
//    }

//    [Fact]
//    public void When_ElementPurchasedEvent_NotExists_Then_ValidationError()
//    {
//        // arrange
//        var dto = new ElementPurchaseCancelledEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id,
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();
//        var entities = _testDbContext.ElementPurchaseCancelledEvents.ToList();

//        // assert
//        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
//        Assert.Empty(entities);
//    }

//    [Fact]
//    public void When_ElementDeliveredEvent_Exists_Then_ValidationError()
//    {
//        // arrange
//        var deliveredEvent = new ElementDeliveredEvent
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        };
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.ElementDeliveredEvents.Add(deliveredEvent);
//        _testDbContext.SaveChanges();

//        var dto = new ElementPurchaseCancelledEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id,
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();
//        var entities = _testDbContext.ElementPurchaseCancelledEvents.ToList();

//        // assert
//        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
//        Assert.Empty(entities);
//    }

//    [Fact]
//    public void IsIdempotent()
//    {
//        // arrange
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.SaveChanges();

//        var dto = new ElementPurchaseCancelledEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id,
//        };

//        // act
//        var results = new[]
//        {
//            _handler.Handle(dto).GetAwaiter().GetResult(),
//            _handler.Handle(dto).GetAwaiter().GetResult(),
//        };
//        var entities = _testDbContext.ElementPurchaseCancelledEvents.ToList();

//        // assert
//        results.Every(x => Assert.Equal(eHandlerResultStatus.Ok, x.Status));
//        Assert.Single(entities);
//    }
//}
