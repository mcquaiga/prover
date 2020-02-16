using System.Threading.Tasks;
using Shared.Domain;

namespace Shared.Interfaces
{
    public interface IRepository<T>
        where T : IEntity
    {
        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}