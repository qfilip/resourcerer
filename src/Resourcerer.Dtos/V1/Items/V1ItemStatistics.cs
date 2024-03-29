﻿namespace Resourcerer.Dtos.V1;

public class V1ItemStatistics : IDto
{
    public Guid ElementId { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public double UnitsPurchased { get; set; }
    public double PurchaseCosts { get; set; }
    public double AveragePurchaseDiscount { get; set; }
    public double UnitsSold { get; set; }
    public double SalesEarning { get; set; }
    public double AverageSaleDiscount { get; set; }
    public double UnitsUsedInComposites { get; set; }
    public int UsedInComposites { get; set; }
    public double UnitsInStock { get; set; }
}
