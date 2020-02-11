using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shared.Domain;

namespace Shared.Interfaces
{
    public interface IAsyncRepository<in TId, T> 
        where TId : struct
        where T : BaseEntity
    {
        #region Methods

        Task<T> AddAsync(T entity);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(T entity);

        Task DeleteAsync(TId id);

        Task<T> GetAsync(TId id);

        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate);

        Task UpdateAsync(T entity);

        #endregion
    }

    public interface IAsyncRepositoryGuid<T> : IAsyncRepository<Guid, T>
        where T : BaseEntity
    {

    }
}