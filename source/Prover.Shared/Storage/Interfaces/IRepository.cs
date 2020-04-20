using System;
using System.Collections.Generic;

namespace Prover.Shared.Storage.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        T Add(T entity);

        bool Update(T entity);

        bool Delete(T entity);

        IEnumerable<T> GetAll();

        T Get<TId>(TId id);
    }
}