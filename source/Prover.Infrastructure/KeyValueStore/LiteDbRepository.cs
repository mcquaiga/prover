using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prover.Shared.Domain;
using Prover.Shared.Interfaces;

namespace Prover.Infrastructure.KeyValueStore
{
    public class LiteDbRepository<T> : IRepository<T>
        where T : class
    {
        protected readonly ILiteDatabase Context;

        public LiteDbRepository(ILiteDatabase context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public T Add(T entity)
        {
            Context.GetCollection<T>().Upsert(entity);
            return entity;
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public T Get(Guid id)
        {
            return Context.GetCollection<T>().FindById(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Context.GetCollection<T>().FindAll();
        }

        public bool Update(T entity)
        {
            return Context.GetCollection<T>().Upsert(entity);
        }
    }

    public class LiteDbAsyncRepository<T> : IAsyncRepository<T>
        where T : AggregateRoot
    {
        protected readonly ILiteDatabase Context;

        public LiteDbAsyncRepository(ILiteDatabase context)
        {
            Context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            var success = Context.GetCollection<T>().Upsert(entity);

            return success ? entity : null;
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                Context.GetCollection<T>().Upsert(entity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
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