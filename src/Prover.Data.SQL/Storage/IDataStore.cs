using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Data.EF.Storage
{
    public interface IDataStore<T> : IDisposable 
        where T : class
    {
        IQueryable<T> Query();
        T Get(Guid id);
        Task<T> UpsertAsync(T entity);
        Task Delete(T entity);
    }
}