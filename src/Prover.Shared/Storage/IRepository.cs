using System;
using System.Linq;
using System.Threading.Tasks;
using Prover.Shared.Domain;

namespace Prover.Shared.Storage
{
    public interface IRepository<T> : IDisposable 
        where T : class
    {
        IQueryable<T> Query();
        Task<T> GetByIdAsync<TId>(TId id);
        Task<T> UpsertAsync(T entity);
        Task DeleteAsync(T entity);
    }
}