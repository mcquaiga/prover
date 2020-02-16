using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shared.Domain;

namespace Shared.Interfaces
{
    public interface IAsyncRepository<in TId, TEntity> : IRepository<TEntity>
        where TEntity : GenericEntity<TId>
    {
        Task DeleteAsync(TId id);
        
        Task<TEntity> GetAsync(TId id);
    }

    public interface IAsyncRepository<T> : IAsyncRepository<Guid, T>
        where T : BaseEntity
    {
        #region Methods

        Task<int> CountAsync(ISpecification<T> spec);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        #endregion
    }

}