using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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
                    .Include(v => v.VerificationTests.Select(vo => vo.VolumeTest));
        }     

        public Instrument Get(Guid id)
        {
            return Query()                  
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<Instrument> UpsertAsync(Instrument entity)
        {
            if (Get(entity.Id) != null)
            {
                _proverContext.Instruments.Attach(entity);
                _proverContext.Entry(entity).State = EntityState.Modified;
            }
            else
                _proverContext.Instruments.Add(entity);

            await _proverContext.SaveChangesAsync();
            return entity;
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