using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Prover.Shared.Domain;
using Prover.Storage.EF.Repositories;
using Prover.Storage.EF.Storage;

namespace Prover.Storage.EF.Tests
{
    [TestFixture]
    public abstract class PersistenceTest<TEntity>
        where TEntity : Entity
    {
        [SetUp]
        public virtual void SetUp()
        {
            GetContext().Database.EnsureCreated();
        }

        private const string SqliteInMemoryConnection = "DataSource=:memory:";
        private const string SqliteFileConnection = "DataSource=prover_data.db";
        private const string SqlServerConnection = @"Server=.\\sqlexpress;Database=Prover_New;Trusted_Connection=True;";
        protected MockRepository MockRepository = new MockRepository(MockBehavior.Default);
        private EfRepository<TEntity> _repository;
        private EfProverContext _context;
        private readonly bool _useSqlite = true;

        protected EfProverContext GetContext()
        {
            return _context ?? DataContextFactory.CreateWithSqlServer(SqlServerConnection);
                   //(_context = _useSqlite
                   //    ? DataContextFactory.CreateWithSqlite(SqliteFileConnection)
                   //    : );
        }

        protected virtual EfRepository<TEntity> SetRepository()
        {
            return new EfRepository<TEntity>(GetContext());
        }

        protected EfRepository<TEntity> Repository
        {
            get
            {
                if (_repository == null) _repository = SetRepository();
                return _repository;
            }
        }

        /// <summary>
        ///     Persistance test helper
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        protected async Task<TEntity> SaveAndLoadEntity(TEntity entity)
        {
            var id = entity.Id;

            await Repository.UpsertAsync(entity);

            return await Repository.GetByIdAsync(id);
        }
    }
}