using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Shared.Data;
using Z.EntityFramework.Plus;

namespace Prover.Core.Storage
{
    public class CertificateStore : ProverStore<Certificate>
    {
        public CertificateStore(ProverContext dbContext) : base(dbContext)
        {
        }     

        protected override IQueryable<Certificate> QueryCommand()
        {
            return Context.Certificates
                .IncludeOptimized(c => c.Instruments);
        }
    }
}