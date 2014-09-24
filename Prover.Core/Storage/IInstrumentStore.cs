using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public interface IInstrumentStore<T> : IDisposable where T : class 
    {
        IQueryable<T> Query();
        IQueryable<Instrument> Get(Guid id);
        void Upsert(T entity);
        void Delete(T entity);
    }
}
