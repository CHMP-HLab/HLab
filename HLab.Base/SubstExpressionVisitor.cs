using System.Collections.Generic;
using System.Linq.Expressions;

namespace HLab.Base;

public class SubstExpressionVisitor : ExpressionVisitor {
    public Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Subst.TryGetValue(node, out var newValue) ? newValue : node;
    }
}