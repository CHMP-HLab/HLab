using System;
using System.Linq.Expressions;

namespace HLab.Base.Extensions;

public static class ExpressionsExtensions
{
    public static Expression<Func<T,TCast>> CastReturn<T,TR,TCast>(this Expression<Func<T,TR>> expr, TCast target)
    {
        var body = expr.Body;
        var t = expr.Body.Type;
        if ((t.IsValueType && t!=typeof(TCast)) || !(t.IsAssignableTo(typeof(TCast))))
        {
            body = Expression.Convert(expr.Body,typeof(TCast));
        }
                
        return Expression.Lambda <Func<T, TCast>>(body, expr.Parameters);
    }

}