using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Data;

namespace Prover.Core.Storage
{   
    public class InstrumentStore : ProverStore<Instrument>
    {
        public InstrumentStore(ProverContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Instrument> Get(Guid id)
        {
            return await QueryCommand().FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Instrument> QueryCommand()
        {
            return Context.Instruments
                .Include(v => v.VerificationTests.Select(t => t.TemperatureTest))
                .Include(v => v.VerificationTests.Select(p => p.PressureTest))
                .Include(v => v.VerificationTests.Select(vo => vo.VolumeTest));
        }

        public override async Task<bool> Delete(Instrument entity)
        {
            entity.ArchivedDateTime = DateTime.UtcNow;
            await Upsert(entity);
            return true;
        }
    }
}