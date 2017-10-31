using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Shared.Data;

namespace Prover.Core.Storage
{
    public class CertificateStore : EfProverStore<Certificate>
    {    
        public CertificateStore(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }

        protected override IQueryable<Certificate> Query()
        {
            return Context.Certificates
                .Include(c => c.Instruments);
        }
    }
}