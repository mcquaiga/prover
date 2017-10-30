using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Storage
{
    public interface IProverStore<T> where T : Entity
    {
        IQueryable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        Task<T> Get(Guid id);
        Task<T> Upsert(T entity);
        Task<bool> Delete(T entity);
    }

    public class ProverStore<T> : IProverStore<T> where T : Entity
    {
        protected ProverContext Context { get; }

        public ProverStore(ProverContext dbContext)
        {
            Context = dbContext;
        }

        public IQueryable<T> GetAll()
        {
            return QueryCommand();
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return QueryCommand().Where(predicate);
        }

        public virtual async Task<T> Get(Guid id)
        {
            return await QueryCommand()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<T> Upsert(T entity)
        {
            if (await Get(entity.Id) != null)
            {
                Context.Set<T>().Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                Context.Set<T>().Add(entity);
            }

            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            return await Context.SaveChangesAsync() > 0;
        }

        protected virtual IQueryable<T> QueryCommand()
        {
            return Context.Set<T>();
        } 
    }
}