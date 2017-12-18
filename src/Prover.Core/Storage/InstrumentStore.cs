using Prover.Core.Models.Instruments;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

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
                .IncludeOptimized(v => v.VerificationTests)
                .IncludeOptimized(v => v.VerificationTests.Select(t => t.TemperatureTest))
                .IncludeOptimized(v => v.VerificationTests.Select(p => p.PressureTest))
                .IncludeOptimized(v => v.VerificationTests.Select(vo => vo.VolumeTest))
                .IncludeOptimized(v => v.VerificationTests.Select(fr => fr.FrequencyTest));
        }

        public override async Task<bool> Delete(Instrument entity)
        {
            entity.ArchivedDateTime = DateTime.UtcNow;
            await Upsert(entity);
            return true;
        }
    }
}