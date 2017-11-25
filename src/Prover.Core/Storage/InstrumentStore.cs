using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
                .Include(v => v.VerificationTests.Select(fr => fr.FrequencyTest))
                .AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            var i = Query().FirstOrDefault(x => x.Id == id);
            
            i.VerificationTests = _proverContext.VerificationTests.Where(v => v.InstrumentId == i.Id).ToList();

            return i;
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