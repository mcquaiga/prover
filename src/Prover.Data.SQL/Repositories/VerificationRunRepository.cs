using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Storage;
using Prover.Domain.Model.Instrument;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Data;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Repositories
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
