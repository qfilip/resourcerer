﻿namespace Resourcerer.DataAccess.Entities;

public class Composite : EntityBase
{
    public Composite()
    {
        OldPrices = new HashSet<OldPrice>();
        Excerpts = new HashSet<Excerpt>();
        CompositeSoldEvents = new HashSet<CompositeSoldEvent>();
    }

    public string? Name { get; set; }
    public double CurrentSellPrice { get; set; }

    public Guid CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    
    public ICollection<Excerpt> Excerpts { get; set; }
    public ICollection<OldPrice> OldPrices { get; set; }
    public ICollection<CompositeSoldEvent> CompositeSoldEvents { get; set; }
}

