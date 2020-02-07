#region

using System;
using System.Linq.Expressions;

#endregion

namespace Prover.Core.Shared.Extensions
{
    public static class ExpressionHelpers
    {
        public static TProperty GetValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression)
            where T : class
        {
            if (obj == null)
                return default(TProperty);

            var func = expression.Compile();

            return func(obj);
        }
    }
}