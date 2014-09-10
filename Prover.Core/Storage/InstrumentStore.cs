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
    public class InstrumentStore : IInstrumentStore<Instrument>, IDisposable
    {
        private readonly ProverContext _proverContext = new ProverContext();

        public IQueryable<Instrument> Query()
        {
            return _proverContext.Instruments.AsQueryable();
        }

        public IQueryable<Instrument> Get(Guid id)
        {
            return _proverContext.Instruments.Where(x => x.Id == id).Include(temp => temp.Temperature);
        }

        public void Save(Instrument entity)
        {
            _proverContext.Instruments.Add(entity);
            _proverContext.Temperatures.Add(entity.Temperature);
            entity.Temperature.Tests.ForEach(test => _proverContext.TemperatureTests.Add(test));
                
            _proverContext.SaveChanges();
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
