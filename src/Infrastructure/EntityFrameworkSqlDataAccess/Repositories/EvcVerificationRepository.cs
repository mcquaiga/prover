using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.EvcVerifications;
using Infrastructure.EntityFrameworkSqlDataAccess.Entities;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Repositories
{
    public class EvcVerificationSpecificationEvaluator : ISpecification<EvcVerificationTest>
    {
        #region Public Properties

        public Expression<Func<EvcVerificationTest, bool>> Criteria { get; }
        public List<Expression<Func<EvcVerificationTest, object>>> Includes { get; }
        public List<string> IncludeStrings { get; }
        public bool isPagingEnabled { get; }
        public Expression<Func<EvcVerificationTest, object>> OrderBy { get; }
        public Expression<Func<EvcVerificationTest, object>> OrderByDescending { get; }
        public int Skip { get; }
        public int Take { get; }

        #endregion
    }

    public class EvcVerificationRepository : IAsyncRepositoryGuid<EvcVerificationTest>
    {
        private readonly ProverDbContext _context;

        public EvcVerificationRepository(ProverDbContext context)
        {
            _context = context;
        }

        #region Public Methods

        public async Task<EvcVerificationTest> AddAsync(EvcVerificationTest entity)
        {
            await _context.EvcVerifications.AddAsync((EvcVerificationSql) entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return await ApplyPredicate(predicate).CountAsync();
        }

        public async Task DeleteAsync(EvcVerificationTest entity)
        {
            if (_context.Entry((EvcVerificationSql) entity).State == EntityState.Detached)
                _context.EvcVerifications.Attach((EvcVerificationSql) entity);

            _context.EvcVerifications.Remove((EvcVerificationSql) entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.EvcVerifications.FindAsync(new[] {id});
            if (entity != null)
                await DeleteAsync(entity);
        }

        public async Task<EvcVerificationTest> GetAsync(Guid id)
        {
            return await _context.EvcVerifications.FindAsync(new[] {id});
        }

        public async Task<IReadOnlyList<EvcVerificationTest>> ListAsync(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return await ApplyPredicate(predicate).ToListAsync();
        }

        public async Task UpdateAsync(EvcVerificationTest entity)
        {
            _context.Attach((EvcVerificationSql) entity);
            _context.Entry((EvcVerificationSql) entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Private

        private IQueryable<EvcVerificationTest> ApplyPredicate(Expression<Func<EvcVerificationTest, bool>> predicate)
        {
            return _context.EvcVerifications.AsQueryable().Where(predicate);
        }

        private IQueryable<EvcVerificationTest> ApplySpecification(ISpecification<EvcVerificationTest> spec)
        {
            var query = _context.EvcVerifications.AsQueryable();

            return SpecificationEvaluator<EvcVerificationTest>.GetQuery(query, spec);
        }

        #endregion
    }
}