using System.Collections.Generic;

namespace Prover.Shared.Storage.Interfaces
{
    public interface IKeyValueStore
    {
        void AddOrUpdate<T>(string key, T value);
        void AddOrUpdate<T>(T value);
        bool Delete<T>(T value);
        bool Delete<T>(string key);
        bool DeleteAll<T>();

        T Get<T>(string key);
        IEnumerable<T> GetAll<T>();
    }
}