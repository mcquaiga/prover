using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;
using Z.EntityFramework.Plus;

namespace Prover.Core.Storage
{
    public class InstrumentStore : IInstrumentStore<Instrument>
    {
        private readonly ProverContext _proverContext;

        public InstrumentStore(ProverContext context)
        {
            _proverContext = context;
        }

        public IQueryable<Instrument> Query()
        {
            return _proverContext.Instruments
                .IncludeOptimized(v => v.VerificationTests)
                .IncludeOptimized(v => v.VerificationTests.Select(vt => vt.VolumeTest))
                .AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            var i = Query()
                .FirstOrDefault(x => x.Id == id);

            i.VerificationTests = _proverContext.VerificationTests
                .IncludeOptimized(v => v.TemperatureTest)
                .IncludeOptimized(v => v.PressureTest)
                .IncludeOptimized(v => v.VolumeTest)
                .IncludeOptimized(v => v.FrequencyTest)
                .Where(v => v.InstrumentId == i.Id)
            .ToList();

            return i;
        }

        public IEnumerable<Instrument> GetAll(Predicate<Instrument> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<Instrument> UpsertAsync(Instrument instrument)
        {
            if (await _proverContext.Instruments.FindAsync(instrument.Id) != null)
            {
                _proverContext.Instruments.Attach(instrument);
                _proverContext.Entry(instrument).State = EntityState.Modified;
            }
            else
                _proverContext.Instruments.Add(instrument);

            await _proverContext.SaveChangesAsync();
            return instrument;
        }

        public async Task Delete(Instrument entity)
        {
            entity.ArchivedDateTime = DateTime.UtcNow;
            await UpsertAsync(entity);
        }

        public void Dispose()
        {
        }
    }
}