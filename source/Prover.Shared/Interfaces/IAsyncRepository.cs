using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Shared.Domain;

namespace Prover.Shared.Interfaces
{
    public interface IAsyncRepository<in TId, TEntity>
        where TEntity : GenericEntity<TId>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task DeleteAsync(TId id);

        Task DeleteAsync(TEntity entity);

        Task<TEntity> GetAsync(TId id);

        Task UpdateAsync(TEntity entity);
    }

    public interface IAsyncRepository<T> : IAsyncRepository<Guid, T>
        where T : AggregateRoot
    {
        Task<int> CountAsync(ISpecification<T> spec);

        IObservable<T> List(Expression<Func<T, bool>> predicate = null);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync();
    }
}