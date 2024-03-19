using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Utilities;
internal sealed class ExpressionUtils
{
    // https://stackoverflow.com/questions/6180704/combine-several-similar-select-expressions-into-a-single-expression

    /*
        Prevent loading json fields, unless specified. DO NOT specify a field where json is mapped to. Ex:
        wrong: x => new Instance() { ReservedEvents = x.ReservedEvents }
        correct: x => new Instance() { ReservedEventsJson = x.ReservedEventsJson }
     */

    public static Expression<Func<T, T>> Combine<T>(params Expression<Func<T, T>>[] selectors)
    {
        var zeroth = ((MemberInitExpression)selectors[0].Body);
        var param = selectors[0].Parameters[0];
        List<MemberBinding> bindings = new List<MemberBinding>(zeroth.Bindings.OfType<MemberAssignment>());
        for (int i = 1; i < selectors.Length; i++)
        {
            var memberInit = (MemberInitExpression)selectors[i].Body;
            var replace = new ParameterReplaceVisitor(selectors[i].Parameters[0], param);
            foreach (var binding in memberInit.Bindings.OfType<MemberAssignment>())
            {
                bindings.Add(Expression.Bind(binding.Member,
                    replace.VisitAndConvert(binding.Expression, "Combine")));
            }
        }


        return Expression.Lambda<Func<T, T>>(
            Expression.MemberInit(zeroth.NewExpression, bindings), param);
    }
}

internal sealed class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly ParameterExpression from, to;
    public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
    {
        this.from = from;
        this.to = to;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == from ? to : base.VisitParameter(node);
    }
}