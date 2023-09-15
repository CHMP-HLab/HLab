using System;
using System.Linq.Expressions;

namespace HLab.Base;

public static class PredicateBuilder {

    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        if (a == null) return b;
        if (b == null) return a;

        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor {Subst = {[b.Parameters[0]] = p}};

        Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body) ?? throw new InvalidOperationException());
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b) {    
        if (a == null) return b;
        if (b == null) return a;

        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor
        {
            Subst = {[b.Parameters[0]] = p}
        };

        Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body) ?? throw new InvalidOperationException());
        return Expression.Lambda<Func<T, bool>>(body, p);
    }   
}