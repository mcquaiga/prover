using Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        #region Methods

        Task<T> AddAsync(T entity);

        Task<int> CountAsync(ISpecification<T> spec);

        Task DeleteAsync(T entity);

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> ListAllAsync();

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task UpdateAsync(T entity);

        #endregion
    }
}