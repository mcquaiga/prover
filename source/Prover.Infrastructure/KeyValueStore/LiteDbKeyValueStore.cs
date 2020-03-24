using System;
using System.Collections.Generic;
using LiteDB;
using Prover.Shared.Interfaces;

namespace Prover.Infrastructure.KeyValueStore
{
    public class LiteDbKeyValueStore : IKeyValueStore
    {
        protected ILiteDatabase Context;

        public LiteDbKeyValueStore(ILiteDatabase context) => Context = context;

        #region IKeyValueStore Members

        public void AddOrUpdate<T>(string key, T value)
        {
            GetSet<T>().Upsert(key, value);
        }

        public void AddOrUpdate<T>(T value)
        {
            GetSet<T>().Upsert(value);
        }

        public bool Delete<T>(T value) => throw new NotImplementedException();

        public bool Delete<T>(string key) => GetSet<T>().Delete(key);

        public bool DeleteAll<T>() => Context.DropCollection(typeof(T).Name);

        public T Get<T>(string key) => GetSet<T>().FindById(key);

        public IEnumerable<T> GetAll<T>() => GetSet<T>().FindAll();

        #endregion

        protected virtual ILiteCollection<T> GetSet<T>() => Context.GetCollection<T>();
    }
}

//public class LiteDbKeyValueRepository<T> : IKeyValueRepository<T>
//{
//    protected ILiteDatabase Context;

//    public LiteDbKeyValueRepository(ILiteDatabase context) => Context = context;

//    #region IKeyValueRepository<T> Members

//    public virtual void AddOrUpdate(string key, T entity)
//    {
//        var col = GetSet();
//        col.Upsert(key, entity);
//    }

//    public virtual void AddOrUpdate<TKey>(TKey key, T entity)
//    {
//        var bson = new BsonValue(key.ToString());
//        var col = GetSet();
//        col.Upsert(entity);
//    }

//    public bool Delete(string key) => GetSet().Delete(key);

//    public bool DeleteAll() => Context.DropCollection(typeof(T).Name);

//    public T Get(string key) => GetSet().FindById(key);

//    public IEnumerable<T> GetAll() => GetSet().FindAll();

//    #endregion

//    protected virtual ILiteCollection<T> GetSet() => Context.GetCollection<T>(typeof(T).Name);
//}