﻿namespace Resourcerer.Dtos.Entities;

public class PriceDto : EntityDto
{
    public double UnitValue { get; set; }

    public Guid? ItemId { get; set; }
    public ItemDto? Item { get; set; }
}


