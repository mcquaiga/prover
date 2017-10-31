using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Storage
{
    public interface IProverStore<T> where T : Entity
    {
        IQueryable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        Task<T> Get(Guid id);        
        Task<T> Upsert(T entity);
        Task<bool> Delete(T entity);
    }
}