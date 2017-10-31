using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Data;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Storage
{
    public class EfProverStore<T> : IProverStore<T> where T : Entity
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        protected ProverContext Context
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<ProverContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext of type UserManagementDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        public EfProverStore(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected virtual IQueryable<T> Query()
        {
            return Context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return Query();
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return Query().Where(predicate);
        }

        public virtual async Task<T> Get(Guid id)
        {
            return await Query()
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

            return entity;
        }

        public virtual async Task<bool> Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            return true;
        }


    }
}