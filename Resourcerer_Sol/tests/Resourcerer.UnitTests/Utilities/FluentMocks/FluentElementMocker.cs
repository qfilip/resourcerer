using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.FluentMocks;

public interface IFluentElementMocker
{
    IFluentElementMocker AddElementSoldEvents(List<(double priceByUnit, int unitsSold)> parameters);
    IFluentElementMocker AddElementPurchasedEvents(List<(double priceByUnit, int unitsBought)> parameters);
    IFluentElementMocker AddExcerpts(List<(Guid compositeId, double quantity)> parameters);
    IFluentElementMocker AddUnitOfMeasure(string name, string symbol);

    Element Build();
}

public class FluentElementMocker : IFluentElementMocker
{
    private Element _element;
    private FluentElementMocker(string name)
    {
        _element = new Element
        {
            Id = Guid.NewGuid(),
            Name = name
        };
    }

    private FluentElementMocker(Guid id, string name)
    {
        _element = new Element
        {
            Id = id,
            Name = name
        };
    }

    public static IFluentElementMocker Create(string name) => new FluentElementMocker(name);
    public static IFluentElementMocker Create(Guid id, string name) => new FluentElementMocker(id, name);

    public IFluentElementMocker AddElementSoldEvents(List<(double priceByUnit, int unitsSold)> parameters)
    {
        parameters
            .Select(x => new ElementSoldEvent
            {
                ElementId = _element.Id,
                PriceByUnit = x.priceByUnit,
                UnitsSold = x.unitsSold
            })
            .ToList()
            .ForEach(_element.ElementSoldEvents.Add);

        return this;
    }

    public IFluentElementMocker AddElementPurchasedEvents(List<(double priceByUnit, int unitsBought)> parameters)
    {
        parameters
            .Select(x => new ElementPurchasedEvent
            {
                ElementId = _element.Id,
                PriceByUnit = x.priceByUnit,
                UnitsBought = x.unitsBought
            })
            .ToList()
            .ForEach(_element.ElementPurchasedEvents.Add);

        return this;
    }

    public IFluentElementMocker AddExcerpts(List<(Guid compositeId, double quantity)> parameters)
    {
        parameters
            .Select(x => new Excerpt
            {
                ElementId = _element.Id,
                CompositeId = x.compositeId,
                Quantity = x.quantity
            })
            .ToList()
            .ForEach(_element.Excerpts.Add);
        
        return this;
    }

    public IFluentElementMocker AddUnitOfMeasure(string name, string symbol)
    {
        _element.UnitOfMeasure = new UnitOfMeasure
        {
            Name = name,
            Symbol = symbol
        };

        return this;
    }

    public Element Build() => _element;
}
