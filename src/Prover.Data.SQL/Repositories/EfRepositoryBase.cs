using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Data.EF.Storage;

namespace Prover.Data.EF.Repositories
{
    public abstract class EfRepositoryBase<T> where T : class
    {
        public IDatabaseFactory DatabaseFactory { get; }
        private readonly DbSet<T> dbset;
        private SqlDataContext _dataContext;

        protected EfRepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = _dataContext.Set<T>();
        }

        protected SqlDataContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.Get());
    }

    public interface IDatabaseFactory
    {
        SqlDataContext Get();
    }
}
