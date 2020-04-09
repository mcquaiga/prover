
using System;
using System.Collections.Generic;

namespace Prover.Shared.Interfaces
{
    public interface IRepository<in TId, T>
        where T : class
    {
        T Add(T entity);

        bool Update(T entity);

        bool Delete(T entity);

        IEnumerable<T> GetAll();

        T Get(TId id);
    }

    public interface IRepository<T> : IRepository<Guid, T>
        where T : class
    {

    }
}