using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Data.SQL.Common;
using Prover.Shared.Domain;

namespace Prover.Data.SQL.Storage
{
    public class SqlRepository<TDto, TId, TDao> : IRepository<TDto, TId> 
        where TDto : Entity<TId> where TDao : class
    {
        private readonly DbContext _dataContext;

        public SqlRepository(DbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TDto> Query()
        {
            IQueryable<TDao> dbQuery = _dataContext.Set<TDao>();
            return dbQuery.AsQueryable().ProjectTo<TDto>();
        }

        public async Task<TDto> Get(Guid id)
        {
            using (var context = new SqlDataContext())
            {
                var result = await context.FindAsync<TDao>(id);

                return Mapper.Map<TDto>(result);
            }
        }

        Task<TDto> IRepository<TDto>.UpsertAsync(TDto entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TDto entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<TDto, TId>.Delete(TDto aggregate)
        {
            throw new NotImplementedException();
        }

        public void Insert(TDto aggregate)
        {
            throw new NotImplementedException();
        }

        public void Update(TDto aggregate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TDto> FindAll()
        {
            throw new NotImplementedException();
        }

        public TDto FindBy(TId id)
        {
            throw new NotImplementedException();
        }
    }
}
