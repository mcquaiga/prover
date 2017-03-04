using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Prover.Data.SQL.Common;
using Prover.Shared.Common;
using Prover.Shared.Storage;

namespace Prover.Data.SQL.Storage
{
    public class SqlRepository<TDto, TDao> : IRepository<TDto> 
        where TDto : Entity
        where TDao : class
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
    }
}
