using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Core.Storage;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Prover.Web.API.Storage
{
    public class InstrumentRepository : IRepository<Instrument>
    {
        private readonly DataContext _proverContext;

        public InstrumentRepository(DataContext context)
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
            return Query().FirstOrDefault(x => x.Id == id);
        }

        public async Task<Instrument> UpsertAsync(Instrument instrument)
        {
            if (Get(instrument.Id) != null)
            {
                _proverContext.Instruments.Attach(instrument);
                _proverContext.Entry(instrument).State = EntityState.Modified;
            }
            else
            {
                _proverContext.Instruments.Add(instrument);
            }

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