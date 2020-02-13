using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shared.Domain;

namespace Shared.Interfaces
{
    public interface IAsyncRepository<T>
        where T : BaseEntity
    {
        #region Methods

        Task<T> AddAsync(T entity);

        Task<int> CountAsync(ISpecification<T> spec);

        Task DeleteAsync(T entity);

        Task DeleteAsync(Guid id);

        Task<T> GetAsync(Guid id);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task UpdateAsync(T entity);

        #endregion
    }

}