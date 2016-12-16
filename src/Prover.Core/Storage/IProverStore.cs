using System;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public interface IProverStore<T> : IDisposable where T : class
    {
        IQueryable<T> Query();
        T Get(Guid id);
        Task<T> UpsertAsync(T entity);
        Task Delete(T entity);
    }
}