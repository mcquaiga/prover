
using System.Collections.Generic;

namespace Prover.Shared.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        T Add(T entity);

        bool Update(T entity);

        bool Delete(T entity);

        IEnumerable<T> GetAll();
    }
}