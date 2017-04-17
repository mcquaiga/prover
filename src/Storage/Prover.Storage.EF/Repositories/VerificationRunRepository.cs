using System.Linq;
using Microsoft.EntityFrameworkCore;
using Prover.Domain.Model.Verifications;

namespace Prover.Storage.EF.Repositories
{
    public class VerificationRunRepository : EfRepository<VerificationRun>
    {
        public VerificationRunRepository(DbContext context) : base(context)
        {
        }

        public override IQueryable<VerificationRun> Table
        {
            get
            {
                return Entities.AsQueryable()
                    .Include(vr => vr.Instrument)
                    .Include(vr => vr.TestPoints)
                    .ThenInclude(tp => tp.Pressure)
                    .Include(vr => vr.TestPoints)
                    .ThenInclude(tp => tp.Temperature)
                    .Include(vr => vr.TestPoints)
                    .ThenInclude(vr => vr.Volume);
            }
        }
    }
}