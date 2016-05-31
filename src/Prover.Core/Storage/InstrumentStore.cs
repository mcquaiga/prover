using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public class InstrumentStore : IInstrumentStore<Instrument>
    {
        private ProverContext _proverContext;
        public InstrumentStore(IUnityContainer container)
        {
            _proverContext = container.Resolve<ProverContext>();
        }

        public IQueryable<Instrument> Query()
        {
            return _proverContext.Instruments
                        .Include(v => v.VerificationTests)
                        .AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            return _proverContext.Instruments.FirstOrDefault(x => x.Id == id);
        }

        public async Task<Instrument> UpsertAsync(Instrument instrument)
        {
            if (this.Get(instrument.Id) != null)
            {
                _proverContext.Instruments.Attach(instrument);
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
            _proverContext.Instruments.Remove(entity);
            await _proverContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            
        }
    }
}
