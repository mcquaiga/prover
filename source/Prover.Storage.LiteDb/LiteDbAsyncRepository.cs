using LiteDB;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Storage.LiteDb
{
    public class LiteDbAsyncRepository<T> : IAsyncRepository<T>
            where T : AggregateRoot
    {
        protected readonly ILiteDatabase Context;
        private ILiteCollection<T> _collection;

        public LiteDbAsyncRepository(ILiteDatabase context)
        {
            Context = context;
            _collection = Context.GetCollection<T>();


        }

        public async Task<T> UpsertAsync(T entity)
        {
            var success = _collection.Upsert(entity);

            return success ? entity : null;
        }

        public Task<int> CountAsync(ISpecification<T> spec) => throw new NotImplementedException();

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate = null)
        {
            predicate = null;
            var results = predicate != null ? _collection.Find(predicate) : Context.GetCollection<T>().FindAll();
            return await Task.FromResult(results);
        }

        public Task DeleteAsync(T entity) => throw new NotImplementedException();

        public Task DeleteAsync(Guid id) => throw new NotImplementedException();

        public async Task<T> GetAsync(Guid id)
        {
            await Task.CompletedTask;

            return _collection.FindById(id);
        }

        public async Task<IReadOnlyList<T>> ListAsync()
        {
            await Task.CompletedTask;
            return _collection.FindAll().ToList();
        }

        //public IEnumerable<T> Query(Expression<Func<T, bool>> predicate = null) => predicate != null
        //        ? Context.GetCollection<T>().Find(predicate)
        //        : Context.GetCollection<T>().FindAll();

        public async Task UpdateAsync(T entity)
        {
            try
            {
                await Observable.StartAsync(async () =>
                {
                    _collection.Upsert(entity);
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}