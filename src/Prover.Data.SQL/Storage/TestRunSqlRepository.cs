using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Models.TestRun;
using Prover.Domain.Verification.TestRun;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Data.EF.Storage
{
    public class TestRunSqlRepository : IRepository<TestRun>
    {
        private readonly SqlDataContext _dataContext;

        internal TestRunSqlRepository(SqlDataContext dataContext)
        {
            _dataContext = dataContext;

            Mapper.Initialize(EfMapperConfiguration.Configure);
        }

        public async Task<TestRun> GetByIdAsync<TId>(TId id)
        {
            var result = await _dataContext.TestRuns.FindAsync(id);
            return Mapper.Map<TestRun>(result);
        }

        public IQueryable<TestRun> Query()
        {
            return _dataContext.TestRuns
                .Include(v => v.TestPoints.Select(t => t.Pressure))
                .Include(v => v.TestPoints.Select(t => t.Temperature))
                .Include(v => v.TestPoints.Select(t => t.Volume))
                .AsQueryable().ProjectTo<TestRun>();
        }

        public async Task<TestRun> UpsertAsync(TestRun entity)
        {
            var testRunDb = Mapper.Map<TestRunDatabase>(entity);

            if (await GetByIdAsync(testRunDb.Id) != null)
            {
                _dataContext.TestRuns.Attach(testRunDb);
                _dataContext.Entry(testRunDb).State = EntityState.Modified;
            }
            else
            {
                _dataContext.TestRuns.Add(testRunDb);
            }
            await _dataContext.SaveChangesAsync();
            return entity;
        }

        public Task DeleteAsync(TestRun entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}