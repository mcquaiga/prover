using System;
using System.Linq;
using System.Threading.Tasks;
using Prover.Shared.Common;

namespace Prover.Shared.Storage
{
    public interface IRepository<T> : IDisposable 
        where T : Entity
    {
        IQueryable<T> Query();
        Task<T> Get(Guid id);
        Task<T> UpsertAsync(T entity);
        Task Delete(T entity);
    }
}
