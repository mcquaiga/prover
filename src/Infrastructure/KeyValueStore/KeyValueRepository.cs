using System;
using LiteDB;
using Shared.Interfaces;

namespace Infrastructure.KeyValueStore
{

    //public class LiteDbSimpleKeyValueRepository : LiteDbKeyValueRepository<string>, IKeyValueRepository
    //{
    //    public LiteDbSimpleKeyValueRepository(ILiteDatabase context) : base(context)
    //    {
    //    }

    //    #region IKeyValueRepository Members

    //    public override void AddOrUpdate(string key, string entity)
    //    {
    //        GetSet().Upsert(key, entity);
    //    }

    //    #endregion

    //    protected override ILiteCollection<string> GetSet() => Context.GetCollection<string>("GlobalKeyValues");
    //}
}

/*
 *
 * public void AddOrUpdate<T>(T entity) where T : IKeyValueEntity
        {
            AddOrUpdate(entity.Key, entity);
        }

        public void AddOrUpdate<T>(string key, T entity)
        {
            var col = GetCol<T>();

            //Check if key/value already exists in collection
            if (col.FindById(key) == null)
                col.Insert(key, entity);
            else
                col.Update(key, entity);
        }

        public bool Delete<T>(T entity) where T : IKeyValueEntity
        {
            return Delete<T>(entity.Key);
        }

        public bool Delete<T>(string key)
        {
            return GetCol<T>().Delete(key);
        }

        public T Get<T>(string key) where T : IKeyValueEntity
        {
            return GetCol<T>().FindById(key);
        }

        public IEnumerable<T> GetAll<T>() where T : IKeyValueEntity
        {
            return GetCol<T>().FindAll();
        }
 *
 */