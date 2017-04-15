using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Shared.Data;
using Prover.Shared.Domain;

namespace Prover.Data.EF.Repositories
{
    /// <summary>
    ///     Entity Framework repository
    /// </summary>
    public class EfRepository<T> : IRepository<T>, IDisposable
        where T : Entity
    {
        protected readonly DbContext Context;
        private DbSet<T> _entities;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public EfRepository(DbContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     Gets a table
        /// </summary>
        public virtual IQueryable<T> Table => Entities.AsQueryable();

        /// <summary>
        ///     Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only
        ///     operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        ///     Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = Context.Set<T>();
                return _entities;
            }
        }

        /// <summary>
        ///     Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Remove(entity);

                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task DeleteAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    Entities.Remove(entity);

                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await Table.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        ///     Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task InsertAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Add(entity);

                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task InsertAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    Entities.Add(entity);

                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                
                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task UpdateAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                entities.ToList().ForEach(e => Context.Entry(e).State = EntityState.Modified);

                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        public virtual async Task UpsertAsync(T entity)
        {
            if (await GetByIdAsync(entity.Id) == null)
                await InsertAsync(entity);
            else
                await UpdateAsync(entity);
        }

        /// <summary>
        ///     Get full error
        /// </summary>
        /// <param name="exc">Exception</param>
        /// <returns>Error</returns>
        protected string GetFullErrorText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
            foreach (var error in validationErrors.ValidationErrors)
                msg += $"Property: {error.PropertyName} Error: {error.ErrorMessage}" +
                       Environment.NewLine;
            return msg;
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> searchPredicate)
        {
            return await Table.Where(searchPredicate).ToListAsync();
        }
    }
}