﻿using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Mocks;
public class DatabaseData
{
    public List<AppUser> AppUsers { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Excerpt> Excerpts { get; set; } = new();
    public List<UnitOfMeasure> UnitsOfMeasure { get; set; } = new();
    public List<Price> Prices { get; set; } = new();
    public List<Composite> Composites { get; set; } = new();
    public List<CompositeSoldEvent> CompositeSoldEvents { get; set; } = new();
    public List<Element> Elements { get; set; } = new();
    public List<ElementInstanceSoldEvent> ElementSoldEvents { get; set; } = new();
    public List<ElementPurchasedEvent> ElementPurchasedEvents { get; set; } = new();
}
