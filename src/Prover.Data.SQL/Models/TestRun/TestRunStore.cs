using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Common;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Storage;
using Prover.Shared.DTO.TestRuns;

namespace Prover.Data.EF.Models.TestRun
{
    internal class TestRunStore : IDataStore<TestRunDto>
    {
        private readonly ProverDbContext _proverContext;

        public TestRunStore(ProverDbContext context)
        {
            _proverContext = context;
            Mapper.Initialize(EfMapperConfiguration.Configure);
        }

        public IQueryable<TestRunDto> Query()
        {
            return _proverContext.TestRuns.AsQueryable().ProjectTo<TestRunDto>(); ;
        }

        public TestRunDto Get(Guid id)
        {
            return Query().FirstOrDefault(x => x.Id == id);
        }

        public async Task<TestRunDto> UpsertAsync(TestRunDto entity)
        {
            var dbEntity = Mapper.Map<TestRunDao>(entity);

            if (Get(entity.Id) != null)
            {
                _proverContext.TestRuns.Attach(dbEntity);
                _proverContext.Entry(dbEntity).State = EntityState.Modified;
            }
            else
            {
                _proverContext.TestRuns.Add(dbEntity);
            }

            await _proverContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(TestRunDto entity)
        {
            var dbEntity = Mapper.Map<TestRunDao>(entity);

            if (Get(entity.Id) != null)
            {
                entity.ArchivedDateTime = DateTime.UtcNow;
                await UpsertAsync(entity);
            }
        }

        public void Dispose()
        {
        }
    }
}