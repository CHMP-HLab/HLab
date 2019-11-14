﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HLab.Base
{
    public static class PredicateBuilder {

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b) {    

            var p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor {Subst = {[b.Parameters[0]] = p}};

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body) ?? throw new InvalidOperationException());
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b) {    

            var p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor
            {
                Subst = {[b.Parameters[0]] = p}
            };

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body) ?? throw new InvalidOperationException());
            return Expression.Lambda<Func<T, bool>>(body, p);
        }   
    }

    internal class SubstExpressionVisitor : ExpressionVisitor {
        public Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Subst.TryGetValue(node, out var newValue) ? newValue : node;
        }
    }


}
