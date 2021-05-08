using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Base.Extensions
{
    public static class ExpressionsExtensions
    {
        public static Expression<Func<T,TCast>> CastReturn<T,TR,TCast>(this Expression<Func<T,TR>> expr, TCast target)
        {
            var param = expr.Parameters[0];
            var body = Expression.Convert(expr.Body,typeof(TCast));
            return Expression.Lambda <Func<T, TCast>>(body, param);
        }

    }
}
