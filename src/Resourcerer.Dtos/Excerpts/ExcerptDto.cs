﻿using FluentValidation;

namespace Resourcerer.Dtos;

public class ExcerptDto : IBaseDto
{
    public Guid CompositeId { get; set; }
    public Guid ElementId { get; set; }

    public ItemDto? Element { get; set; }

    public double Quantity { get; set; }

    public class Validator : AbstractValidator<ExcerptDto>
    {
        public Validator()
        {
            RuleFor(x => x.CompositeId)
                .NotEmpty()
                .WithMessage("CompositeId cannot be empty guid");

            RuleFor(x => x.ElementId)
                .NotEmpty()
                .WithMessage("ElementId cannot be empty guid");
        }
    }
}