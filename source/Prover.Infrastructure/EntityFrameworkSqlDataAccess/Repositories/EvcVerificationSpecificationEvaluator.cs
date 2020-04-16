using System;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Specifications;

namespace Prover.Infrastructure.EntityFrameworkSqlDataAccess.Repositories
{
    public sealed class EvcVerificationSpecificationEvaluator : BaseSpecification<EvcVerificationTest>
    {
        public EvcVerificationSpecificationEvaluator(Guid id) : base(v => v.Id == id)
        {
            AddInclude(v => v.Tests);
            AddIncludes(agg => agg.Include(v => v.Tests));
        }
    }
}