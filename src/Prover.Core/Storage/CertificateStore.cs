using System.Linq;
using Prover.Core.Models.Certificates;
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
                .IncludeOptimized(c => c.Instruments)
                .IncludeOptimized(c => c.Client);
        }
    }
}