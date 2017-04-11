using System;
using System.Linq.Expressions;

namespace Prover.Shared.Extensions
{
    public static class ExpressionHelpers
    {
        public static TProperty GetValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression) where T : class
        {
            if (obj == null)
            {
                return default(TProperty);
            }

            Func<T, TProperty> func = expression.Compile();

            return func(obj);
        }
    }
    
}