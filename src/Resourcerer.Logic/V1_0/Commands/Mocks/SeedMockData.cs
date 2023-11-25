using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using M = Resourcerer.Logic.Commands.Mocks.Helpers.Mocker;

namespace Resourcerer.Logic.Commands.V1_0;
public static class SeedMockData
{
    public class Handler : IAppHandler<Unit, Unit>
    {
        private readonly AppDbContext ctx;

        public Handler(AppDbContext dbContext)
        {
            ctx = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(Unit unit)
        {
            var adminUser = M.MockUser(ctx, "admin", (x) => x.Name = "admin", true);
            var loserUser = M.MockUser(ctx, "alice", (x) => x.Name = "alice");

            // categories
            var bar = M.MockCategory(ctx, x => x.Name = "Bar");
            var spirits = M.MockCategory(ctx, x =>
            {
                x.Name = "Spirits";
                x.ParentCategoryId = bar.Id;
            });
            var ales = M.MockCategory(ctx, x =>
            {
                x.Name = "Ales";
                x.ParentCategoryId = bar.Id;
            });
            var waters = M.MockCategory(ctx, x =>
            {
                x.Name = "Waters";
                x.ParentCategoryId = bar.Id;
            });
            var veggies = M.MockCategory(ctx, x =>
            {
                x.Name = "Veggies";
                x.ParentCategoryId = bar.Id;
            });
            var cocktails = M.MockCategory(ctx, x =>
            {
                x.Name = "Cocktails";
                x.ParentCategoryId = bar.Id;
            });
            var tikiCocktails = M.MockCategory(ctx, x =>
            {
                x.Name = "Tiki";
                x.ParentCategoryId = cocktails.Id;
            });

            // units of measure
            var liter = M.MockUnitOfMeasure(ctx, x =>
            {
                x.Name = "Liter";
                x.Symbol = "l";
            });
            var kg = M.MockUnitOfMeasure(ctx, x =>
            {
                x.Name = "Kilogram";
                x.Symbol = "kg";
            });

            // elements
            var vodka = M.MockItem(ctx, x =>
            {
                x.Name = "Vodka";
                x.Category = spirits;
                x.UnitOfMeasure = liter;
            }, 8, 1);
            var rum = M.MockItem(ctx, x =>
            {
                x.Name = "Rum";
                x.Category = spirits;
                x.UnitOfMeasure = liter;
            }, 15, 1);
            var gin = M.MockItem(ctx, x =>
            {
                x.Name = "Gin";
                x.Category = spirits;
                x.UnitOfMeasure = liter;
            }, 10, 1);
            var gingerAle = M.MockItem(ctx, x =>
            {
                x.Name = "Ginger ale";
                x.Category = ales;
                x.UnitOfMeasure = liter;
            }, 10, 1);
            var sparklingWater = M.MockItem(ctx, x =>
            {
                x.Name = "Sparkling water";
                x.Category = waters;
                x.UnitOfMeasure = liter;
            }, 10, 1);
            var lime = M.MockItem(ctx, x =>
            {
                x.Name = "Lime";
                x.Category = veggies;
                x.UnitOfMeasure = liter;
            }, 10, 1);

            // composites
            var moscowMule = M.MockItem(ctx, x =>
            {
                x.Name = "Moscow Mule";
                x.Category = cocktails;
                x.UnitOfMeasure = liter;
            }, 5, 1);
            var darkNstormy = M.MockItem(ctx, x =>
            {
                x.Name = "Dark 'n Stormy";
                x.Category = cocktails;
                x.UnitOfMeasure = liter;
            }, 7, 1);
            var ginFizz = M.MockItem(ctx, x =>
            {
                x.Name = "Gin Fizz";
                x.Category = cocktails;
                x.UnitOfMeasure = liter;
            }, 7, 1);

            // excerpts
            var moscowMuleExcerpts = M.MockExcerpts(ctx, moscowMule, new (Item, double)[]
            {
                (vodka, 0.005d), (gingerAle, 0.003d), (lime, 0.025d)
            });
            var darkNStormyExcerpts = M.MockExcerpts(ctx, darkNstormy, new (Item, double)[]
            {
                (rum, 0.005d), (gingerAle, 0.003d), (lime, 0.025d)
            });
            var ginFizzExcerpts = M.MockExcerpts(ctx, ginFizz, new (Item, double)[]
            {
                (gin, 0.005d), (sparklingWater, 0.003d), (lime, 0.025d)
            });

            // remove generated extras
            ctx.Categories.RemoveRange(ctx.Categories.Where(x => x.Name!.StartsWith("test")).ToArray());
            
            await ctx.BaseSaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}

