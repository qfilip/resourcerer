using FluentValidation;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class CreateExample
{
    public class Handler : IAppEventHandler<V1CreateExampleCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateExampleCommand request)
        {
            var entity = new ExampleEntity
            {
                Text = request.Text,
            };

            _dbContext.Examples.Add(entity);
            await _dbContext.SaveChangesAsync();
            
            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }

    public class Validator : AbstractValidator<V1CreateExampleCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Text)
                .Must(Validation.Example.Text)
                .WithMessage(Validation.Example.TextError);
        }
    }
}
