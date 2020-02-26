
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Shared.Domain;

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