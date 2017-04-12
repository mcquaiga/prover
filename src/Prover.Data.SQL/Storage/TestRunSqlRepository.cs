using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Models.TestRun;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Data.EF.Storage
{
    public class TestRunSqlRepository : IRepository<TestRunDto>
    {
        private readonly SqlDataContext _dataContext;

        internal TestRunSqlRepository(SqlDataContext dataContext)
        {
            _dataContext = dataContext;

            Mapper.Initialize(EfMapperConfiguration.Configure);
        }

        public async Task<TestRunDto> GetByIdAsync<TId>(TId id)
        {
            var result = await _dataContext.TestRuns.FindAsync(id);
            return Mapper.Map<TestRunDto>(result);
        }

        public IQueryable<TestRunDto> Query()
        {
            return _dataContext.TestRuns
                .Include(v => v.TestPoints.Select(t => t.Pressure))
                .Include(v => v.TestPoints.Select(t => t.Temperature))
                .Include(v => v.TestPoints.Select(t => t.Volume))
                .AsQueryable().ProjectTo<TestRunDto>();
        }

        public async Task<TestRunDto> UpsertAsync(TestRunDto entity)
        {
        //    var testRun = Mapper.Map<TestRunDatabase>(entity);

        //    if (await GetByIdAsync(testRun.Id) != null)
        //    {
        //        _dataContext.TestRuns.Attach(testRun);
        //        _dataContext.Entry(testRun).State = EntityState.Modified;
        //    }
        //    else
        //    {
        //        _dataContext.TestRuns.Add(testRun);
        //    }
        //    await _dataContext.SaveChangesAsync();
            return entity;
        }

        public Task DeleteAsync(TestRunDto entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}