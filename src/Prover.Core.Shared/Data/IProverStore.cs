using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Shared.Data
{
    public interface IProverStore<T, in TId>
        where T : GenericEntity<TId>
        
    {
        IQueryable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        Task<T> Get(TId id);
        Task<T> Upsert(T entity);
        Task<bool> Delete(T entity);
    }

    public interface IProverStore<T> : IProverStore<T, Guid>
        where T : GenericEntity<Guid>
    {
        
    }
}