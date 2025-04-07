using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class UpdateExample
{
    public class Handler : IAppEventHandler<V1UpdateExampleCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1UpdateExampleCommand request)
        {
            var entity = await _dbContext.Examples
                .Where(x => x.Id == request.ExampleId)
                .FirstOrDefaultAsync();

            if (entity == null)
                return HandlerResult<Unit>.NotFound();

            entity.Text = request.NewText;
            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

    }
    public class Validator : AbstractValidator<V1UpdateExampleCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ExampleId)
                .NotEmpty().WithMessage("Example Id cannot be empty");

            RuleFor(x => x.NewText)
                .Must(Validation.Example.Text)
                .WithMessage(Validation.Example.TextError);
        }
    }
}
