using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using LiteDB;
using Shared.Domain;
using Shared.Interfaces;

namespace Infrastructure.KeyValueStore
{
    public class LiteDbRepository<T> : IAsyncRepository<T>
        where T : AggregateRoot
    {
        protected readonly ILiteDatabase Context;

        public LiteDbRepository(ILiteDatabase context)
        {
            Context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            Context.GetCollection<T>().Upsert(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            Context.GetCollection<T>().Upsert(entity);
        }

        public Task DeleteAsync(T entity) => throw new NotImplementedException();

        public Task DeleteAsync(Guid id) => throw new NotImplementedException();

        public async Task<T> GetAsync(Guid id)
        {
            return Context.GetCollection<T>().FindById(id);
        }

        public Task<int> CountAsync(ISpecification<T> spec) => throw new NotImplementedException();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return Context.GetCollection<T>().Find(spec.Criteria).ToList();
        }

        public async Task<IReadOnlyList<T>> ListAsync()
        {
            return Context.GetCollection<T>().FindAll().ToList();
        }

        public IObservable<T> List(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return Context.GetCollection<T>().FindAll().ToObservable();

            return Context.GetCollection<T>().Find(predicate).ToObservable();
        }
    }
}