using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

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
                .Include(v => v.VerificationTests.Select(t => t.TemperatureTest))
                .Include(v => v.VerificationTests.Select(p => p.PressureTest))
                .Include(v => v.VerificationTests.Select(vo => vo.VolumeTest))
                .AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            return _proverContext.Instruments.FirstOrDefault(x => x.Id == id);
        }

        public async Task<Instrument> UpsertAsync(Instrument instrument)
        {
            if (Get(instrument.Id) != null)
                _proverContext.Instruments.Attach(instrument);
            else
                _proverContext.Instruments.Add(instrument);
            await _proverContext.SaveChangesAsync();
            return instrument;
        }

        public async Task Delete(Instrument entity)
        {
            _proverContext.Instruments.Remove(entity);
            await _proverContext.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}