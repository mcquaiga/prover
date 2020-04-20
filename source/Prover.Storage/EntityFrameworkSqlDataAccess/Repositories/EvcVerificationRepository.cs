using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using Prover.Storage.EntityFrameworkSqlDataAccess.Entities;
using Prover.Storage.EntityFrameworkSqlDataAccess.Storage;

namespace Prover.Storage.EntityFrameworkSqlDataAccess.Repositories
{
    public class EvcVerificationRepository : EfRepository<EvcVerificationTest>
    {
   
        public EvcVerificationRepository(ProverDbContext context) : base(context)
        {
            
        }

        #region Public Methods

        public override async Task<EvcVerificationTest> UpsertAsync(EvcVerificationTest entity)
        {
            await Context.EvcVerifications.AddAsync((EvcVerificationSql)entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public Task<int> CountAsync(ISpecification<EvcVerificationTest> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return await ApplyPredicate(predicate).CountAsync();
        }

        public async Task DeleteAsync(EvcVerificationTest entity)
        {
            if (Context.Entry((EvcVerificationSql)entity).State == EntityState.Detached)
                Context.EvcVerifications.Attach((EvcVerificationSql)entity);

            Context.EvcVerifications.Remove((EvcVerificationSql)entity);

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Context.EvcVerifications.FindAsync(new[] { id });
            if (entity != null)
                await DeleteAsync(entity);
        }

        public async Task<EvcVerificationTest> GetAsync(Guid id)
        {
            return await Context.EvcVerifications
                    .FindAsync(new[] { id });
        }

        public Task<IReadOnlyList<EvcVerificationTest>> ListAsync(ISpecification<EvcVerificationTest> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<EvcVerificationTest>> ListAsync(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return await ApplyPredicate(predicate)
                .ToListAsync();
        }

        public async Task UpdateAsync(EvcVerificationTest entity)
        {
            Context.Attach((EvcVerificationSql)entity);
            Context.Entry((EvcVerificationSql)entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        #endregion

        #region Private

        private IQueryable<EvcVerificationTest> ApplyPredicate(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return Context.EvcVerifications.AsQueryable().Where(predicate);
        }

        private IQueryable<EvcVerificationTest> ApplySpecification(ISpecification<EvcVerificationTest> spec)
        {
            var query = Context.EvcVerifications.AsQueryable();

            return SpecificationEvaluator<EvcVerificationTest>.GetQuery(query, spec);
        }

        #endregion
    }
}