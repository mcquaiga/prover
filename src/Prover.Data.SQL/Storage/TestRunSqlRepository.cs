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
        private readonly TestRunSqlContext _dataContext;

        public TestRunSqlRepository(TestRunSqlContext dataContext)
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

        public Task<TestRunDto> UpsertAsync(TestRunDto entity)
        {
            throw new NotImplementedException();
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
