using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Prover.Shared.Interfaces
{
    public interface ISpecification<T>
    {
        #region Properties

        Expression<Func<T, bool>> Criteria { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        List<string> IncludeStrings { get; }

        bool isPagingEnabled { get; }

        Expression<Func<T, object>> OrderBy { get; }

        Expression<Func<T, object>> OrderByDescending { get; }

        int Skip { get; }

        int Take { get; }

        #endregion
    }
}