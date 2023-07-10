//using Resourcerer.DataAccess.Contexts;
//using Resourcerer.DataAccess.Entities;
//using Resourcerer.Dtos;
//using Resourcerer.Logic;
//using Resourcerer.Logic.Commands.ElementEvents;
//using Resourcerer.UnitTests.Utilities;
//using Resourcerer.UnitTests.Utilities.Mocker;

//namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

//public class CreateElementDeliveredEventTests
//{
//    private readonly AppDbContext _testDbContext;
//    private readonly ElementPurchasedEvent _testPurchasedEvent;
//    private readonly CreateElementDeliveredEvent.Handler _handler;
//    public CreateElementDeliveredEventTests()
//    {
//        _testDbContext = new ContextCreator().GetTestDbContext();
//        _testPurchasedEvent = Mocker.MockElementPurchasedEvent(_testDbContext);
//        _handler = new CreateElementDeliveredEvent.Handler(_testDbContext);
//    }

//    [Fact]
//    public void When_ElementPurchasedEvent_Exists_Then_Ok_And_ElementInstance_Created()
//    {
//        // arrange
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.SaveChanges();

//        var dto = new CreateElementDeliveredEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();
//        var deliveredEventEntities = _testDbContext.ElementDeliveredEvents.ToList();
//        var instanceEntities = _testDbContext.Instances.ToList();

//        // assert
//        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
//        Assert.Single(deliveredEventEntities);
//        Assert.Single(instanceEntities);
//        Assert.Equal(_testPurchasedEvent.ElementId, instanceEntities[0].ElementId);
//    }

//    [Fact]
//    public void When_ElementPurchasedEvent_NotExists_Then_ValidationError()
//    {
//        var dto = new CreateElementDeliveredEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();

//        // assert
//        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
//    }

//    [Fact]
//    public void When_ElementPurchaseCancelledEvent_Exists_Then_ValidationError()
//    {
//        // arrange
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.ElementPurchaseCancelledEvents.Add(new()
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        });
//        _testDbContext.SaveChanges();

//        var dto = new CreateElementDeliveredEventDto()
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        };

//        // act
//        var result = _handler.Handle(dto).GetAwaiter().GetResult();
//        var entites = _testDbContext.ElementDeliveredEvents.ToList();
        
//        // assert
//        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
//        Assert.Empty(entites);
//    }

//    [Fact]
//    public void IsIdempotent()
//    {
//        // arrange
//        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
//        _testDbContext.SaveChanges();

//        var dto = new CreateElementDeliveredEventDto
//        {
//            ElementPurchasedEventId = _testPurchasedEvent.Id
//        };

//        // act
//        var results = new[]
//        {
//            _handler.Handle(dto).GetAwaiter().GetResult(),
//            _handler.Handle(dto).GetAwaiter().GetResult()
//        };
//        var entites = _testDbContext.ElementDeliveredEvents.ToList();

//        // assert
//        results.Every(x => Assert.Equal(eHandlerResultStatus.Ok, x.Status));
//        Assert.Single(entites);
//    }
//}
