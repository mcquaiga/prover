using System;
using System.Collections.Generic;
using LiteDB;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Storage.LiteDb
{
    public class LiteDbRepository<T> : IRepository<T>
        where T : class
    {
        protected readonly ILiteDatabase Context;

        public LiteDbRepository(ILiteDatabase context) =>
            Context = context ?? throw new ArgumentNullException(nameof(context));

        public T Add(T entity)
        {
            Context.GetCollection<T>().Upsert(entity);
            return entity;
        }

        public bool Delete(T entity) => throw new NotImplementedException();

        public virtual T Get<TId>(TId id)
        {
            var value = new BsonValue(id.ToString());
            return Context.GetCollection<T>().FindById(value);
        }

        public IEnumerable<T> GetAll() => Context.GetCollection<T>().FindAll();

        public bool Update(T entity) => Context.GetCollection<T>().Upsert(entity);
    }

    //public class LiteDbRepository<T> : LiteDbRepository<T>, IRepository<T>
    //    where T : class
    //{
    //    /// <inheritdoc />
    //    public LiteDbRepository(ILiteDatabase context) : base(context)
    //    {
    //    }

    //    /// <inheritdoc />
    //    public override T Get(Guid id)
    //    {
    //        return Context.GetCollection<T>().FindById(id);
    //    }
    //}
}