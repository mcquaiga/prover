using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Prover.Domain.Model.Instrument;
using Prover.Shared.Data;
using Prover.Shared.Domain;
using Prover.Storage.EF;
using Prover.Storage.EF.Repositories;

namespace Prover.Data.EF.Tests
{
    [TestFixture]
    public abstract class PersistenceTest<TEntity>
        where TEntity : Entity
    {
        private const string SqliteInMemoryConnection = "DataSource=:memory:";
        private const string SqlServerConnection = "DataSource=:memory:";
        protected MockRepository MockRepository = new MockRepository(MockBehavior.Default);
        private EfRepository<TEntity> _repository;
        private ProverObjectContext _context;

        [SetUp]
        public virtual void SetUp()
        {          
            GetContext().Database.EnsureCreated();
        }
        
        protected ProverObjectContext GetContext()
        {
            if (_context == null)
                _context = DataContextFactory.CreateWithSqlite(SqliteInMemoryConnection);
                //return DataContextFactory.CreateWithSqlServer(SqlServerConnection);
            return _context;
            
        }

        protected virtual EfRepository<TEntity> SetRepository()
        {
            return new EfRepository<TEntity>(GetContext());
        }

        protected EfRepository<TEntity> Repository
        {
            get
            {
                if (_repository == null) _repository = SetRepository() ;
                return _repository;
            }
        }

        /// <summary>
        /// Persistance test helper
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="disposeContext">A value indicating whether to dispose context</param>
        protected async Task<TEntity> SaveAndLoadEntity(TEntity entity)
        {
            var id = entity.Id;

            await Repository.UpsertAsync(entity); 
            
            return await Repository.GetByIdAsync(id);
        }

        protected EvcInstrument GetTestInstrument()
        {
            var instrument = new EvcInstrument()
            {
                InstrumentType = "Mini-Max",
                Items = GetTestItemData()
            };
            
            return instrument;
        }

        protected Dictionary<string, string> GetTestItemData()
        {
            var itemJson =
                "{\"0\":\" 0000098\",\"2\":\" 0008169\",\"5\":\"00000000\",\"6\":\"00000000\",\"8\":\"   80.06\",\"10\":\"  100.00\",\"11\":\"   -1.00\",\"12\":\" 14.4000\",\"13\":\" 14.6500\",\"14\":\" 14.4303\",\"26\":\"   32.30\",\"27\":\"  -30.00\",\"28\":\"  170.00\",\"34\":\"   60.00\",\"35\":\" -0.0543\",\"44\":\"  6.4499\",\"45\":\"  1.0563\",\"47\":\"  1.0069\",\"53\":\"  0.5850\",\"54\":\"  1.6000\",\"55\":\"  0.7000\",\"56\":\"  2.0000\",\"57\":\"  2.0000\",\"62\":\"12019164\",\"87\":\"       0\",\"89\":\"       0\",\"90\":\"       6\",\"92\":\"       5\",\"93\":\"       0\",\"94\":\"       2\",\"98\":\"       3\",\"109\":\"       0\",\"110\":\"       0\",\"111\":\"       0\",\"112\":\"       0\",\"113\":\"098.8073\",\"122\":\"  6.9200\",\"137\":\"  100.00\",\"140\":\" 0000098\",\"141\":\"       1\",\"142\":\" 1000.00\",\"147\":\"       0\",\"200\":\"12019164\",\"201\":\"02549645\"}";
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(itemJson);
        }
    }
}
