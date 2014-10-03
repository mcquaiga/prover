using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public class InstrumentStore : IInstrumentStore<Instrument>
    {
        private readonly ProverContext _proverContext = new ProverContext();

        public IQueryable<Instrument> Query()
        {
            return _proverContext.Instruments.AsQueryable();
        }

        public Instrument Get(Guid id)
        {
            return _proverContext.Instruments.First(x => x.Id == id);
        }

        public Temperature GetTemperature(Guid id)
        {
            return _proverContext.Temperatures.Find(id);
        }

        public void Upsert(Instrument instrument)
        {
            using (var context = new ProverContext())
            {
                context.Instruments.Add(instrument);
                context.SaveChanges();
            }
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
