using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Domain.EvcVerifications;
using Shared.Interfaces;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Repositories
{
    public class EvcVerificationSpecificationEvaluator : ISpecification<EvcVerificationTest>
    {
        #region Public Properties

        public Expression<Func<EvcVerificationTest, bool>> Criteria { get; }
        public List<Expression<Func<EvcVerificationTest, object>>> Includes { get; }
        public List<string> IncludeStrings { get; }
        public bool isPagingEnabled { get; }
        public Expression<Func<EvcVerificationTest, object>> OrderBy { get; }
        public Expression<Func<EvcVerificationTest, object>> OrderByDescending { get; }
        public int Skip { get; }
        public int Take { get; }

        #endregion
    }
}