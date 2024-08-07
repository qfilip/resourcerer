﻿namespace Resourcerer.Dtos.V1;

public class V1CreateElementItem : IDto
{
    public string? Name { get; set; }
    public double ProductionPrice { get; set; }
    public double ProductionTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public double UnitPrice { get; set; }
}
