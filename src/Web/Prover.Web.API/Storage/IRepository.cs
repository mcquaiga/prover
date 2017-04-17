using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Web.API.Storage
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> Query();
        T Get(Guid id);
        Task<T> UpsertAsync(T entity);
        Task Delete(T entity);
    }
}