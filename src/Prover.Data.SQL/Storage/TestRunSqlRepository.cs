using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.SQL.Mappers;
using Prover.Data.SQL.Models;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Data.SQL.Storage
{
    public class TestRunSqlRepository : IRepository<TestRunDto>
    {
        private readonly ProverDbContext _dataContext;

        internal TestRunSqlRepository(ProverDbContext dataContext)
        {
            _dataContext = dataContext;

            Mapper.Initialize(MappingConfiguration.Configure);
        }

        public IQueryable<TestRunDto> Query()
        {
            return _dataContext.TestRuns.AsQueryable().ProjectTo<TestRunDto>();
        }

        public async Task<TestRunDto> Get(Guid id)
        {
            var result = await _dataContext.TestRuns.FindAsync(id);
            return Mapper.Map<TestRunDto>(result);
        }

        public async Task<TestRunDto> UpsertAsync(TestRunDto entity)
        {
            var testRun = Mapper.Map<TestRunDao>(entity);

            if (await Get(testRun.Id) != null)
            {
                _dataContext.TestRuns.Attach(testRun);
                _dataContext.Entry(testRun).State = EntityState.Modified;
            }
            else
            {
                _dataContext.TestRuns.Add(testRun);
            }
            await _dataContext.SaveChangesAsync();
            return entity;
        }

        public Task Delete(TestRunDto entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
