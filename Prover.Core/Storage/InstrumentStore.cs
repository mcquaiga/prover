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
            return _proverContext.Instruments.AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            return _proverContext.Instruments.First(x => x.Id == id);
        }

        public async Task UpsertAsync(Instrument instrument)
        {
            _proverContext.Instruments.Add(instrument);
            await _proverContext.SaveChangesAsync();
        }

        public void Delete(Instrument entity)
        {
            _proverContext.Instruments.Remove(entity);
        }

        public void Dispose()
        {
            
        }
    }
}
