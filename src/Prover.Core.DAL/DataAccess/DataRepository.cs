using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Prover.Core.DAL.Common;
using Prover.Core.DAL.DataAccess.QaTestRuns;
using Prover.Core.Domain.Models.QaTestRuns;

namespace Prover.Core.DAL.DataAccess
{
    public interface IDataRepository<TDal, TDto> 
        where TDal : AggregateRoot 
        where TDto : class
    {
        Task<IEnumerable<TDal>> GetAllAsync(params Expression<Func<TDal, object>>[] navigationProperties);
        Task<TDal> GetByIdAsync(Guid id);
        Task UpsertAsync(TDto dto);
    }

    public class DataRepository<TDal, TDto> : IDataRepository<TDal, TDto> 
        where TDto : class
        where TDal : AggregateRoot
    {
        public async Task<IEnumerable<TDal>> GetAllAsync(params Expression<Func<TDal, object>>[] navigationProperties)
        {
            List<TDal> sourceList;
            using (var context = new DataContext())
            {
                IQueryable<TDal> dbQuery = context.Set<TDal>();

                //Apply eager loading
                foreach (Expression<Func<TDal, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include(navigationProperty);

                sourceList = await dbQuery.ToListAsync();
            }

            //var resultList = Mapper.Map<IEnumerable<TDestination>>(sourceList);

            return sourceList;
        }

        public async Task<TDal> GetByIdAsync(Guid id)
        {
            TDal source;
            using (var context = new DataContext())
            {
                source = await context.Set<TDal>().FindAsync(id);
            }

            //var result = Mapper.Map<TDestination>(source);
            return source;
        }

        public async Task UpsertAsync(TDto dto)
        {
            var dal = Mapper.Map<TDal>(dto);

            using (var context = new DataContext())
            {
                await context.AddAsync(dal);
                await context.SaveChangesAsync();
            }
        }
    }
}
