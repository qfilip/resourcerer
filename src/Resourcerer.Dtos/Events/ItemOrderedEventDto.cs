﻿using FluentValidation;
using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemOrderedEventDto : ItemEventDtoBase
{
    public Guid ItemId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public class Validator : AbstractValidator<ItemOrderedEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty()
                .WithMessage("Item id cannot be empty");

            RuleFor(x => x.UnitsOrdered)
                .GreaterThan(0)
                .WithMessage("Number of ordered units must be greater than 0");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");

            RuleFor(x => x.TotalDiscountPercent)
                .InclusiveBetween(0, 100)
                .WithMessage("Total discount must be greater between in 0 and 100");
        }
    }
}
