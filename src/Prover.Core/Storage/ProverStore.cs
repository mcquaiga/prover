using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Data;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Storage
{
    public class ProverStore<T, TId> : IProverStore<T, TId> 
        where T : GenericEntity<TId>
    {
        public ProverContext Context { get; }

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

        public virtual async Task<T> Get(TId id)
        {
            return await GetById(id);
        }

        public virtual async Task<T> Upsert(T entity)
        {
            var entry = Context.Entry(entity);
            var state = entry.State;

            if (state == EntityState.Detached || state == EntityState.Unchanged)
            {
                var existing = await GetById(entity.Id);
                if (existing == null)
                {
                    Context.Set<T>().Add(entity);
                    Context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    Context.Set<T>().Attach(existing);
                    Context.Entry(existing).State = EntityState.Modified;
                }
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

        protected async Task<T> GetById(TId id)
        {
            return await Context.Set<T>().FindAsync(id);
        }
    }

    public class ProverStore<T> : ProverStore<T, Guid>, IProverStore<T>
        where T : GenericEntity<Guid>
    {
        public ProverStore(ProverContext dbContext) : base(dbContext)
        {
        }
    }
}