using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Shared.Domain;

namespace Prover.Shared.Storage.Interfaces
{
    public interface IAsyncRepository<in TId, TEntity>
        where TEntity : GenericEntity<TId>
    {
        Task<TEntity> UpsertAsync(TEntity entity);

        Task DeleteAsync(TId id);
        Task DeleteAsync(TEntity entity);

        Task<TEntity> GetAsync(TId id);
    }

    public interface IAsyncRepository<T> : IAsyncRepository<Guid, T>
        where T : AggregateRoot
    {
        Task<int> CountAsync(ISpecification<T> spec);

        IEnumerable<T> Query(Expression<Func<T, bool>> predicate = null);

        Task<IReadOnlyList<T>> ListAsync();
    }

    public interface IObservableRepository<T>
        where T : AggregateRoot
    {
        IQbservable<T> QueryObservable(Expression<Func<T, bool>> predicate = null);
    }
}