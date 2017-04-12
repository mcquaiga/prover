using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Mappers;
using Prover.Data.EF.Models.Prover;
using Prover.Data.EF.Storage;
using Prover.Domain.Models.Prover;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Storage;

namespace Prover.Data.EF.Repositories
{
    internal class ProverTestRunRepository : EfRepositoryBase<ProverTestRun>, IRepository<ProverTestRun>
    {
        public ProverTestRunRepository(IDatabaseFactory databaseFactory) : base(databaseFactory)
        {
        }

        public IQueryable<ProverTestRun> Query()
        {
            return DataContext.TestRuns
                .Include(v => v.TestPoints.Select(t => t.Pressure))
                .Include(v => v.TestPoints.Select(t => t.Temperature))
                .Include(v => v.TestPoints.Select(t => t.Volume))
                .AsQueryable()
                .ProjectTo<ProverTestRun>();
        }

        public async Task<ProverTestRun> GetByIdAsync<TId>(TId id)
        {
            var result = await DataContext.TestRuns.FindAsync(id);
            return Mapper.Map<ProverTestRun>(result);
        }

        public async Task<ProverTestRun> UpsertAsync(ProverTestRun entity)
        {
            var dbObject = Mapper.Map<ProverTestRunDatabase>(entity);
            if (await GetByIdAsync(entity.Id) != null)
            {
                DataContext.TestRuns.Attach(dbObject);
                DataContext.Entry(dbObject).State = EntityState.Modified;
            }
            else
            {
                DataContext.TestRuns.Add(dbObject);
            }

            await DataContext.SaveChangesAsync();
            return Mapper.Map<ProverTestRun>(dbObject);
        }

        public Task DeleteAsync(ProverTestRun entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
