using Devices.Core.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Prover.Storage.MongoDb
{

    public class MongoDbInitializer
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<EvcVerificationTest>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(c => c.Device).SetSerializer(new DeviceInstanceBsonSerializer());
            });

            BsonSerializer.RegisterSerializer(typeof(DeviceInstance), new DeviceInstanceBsonSerializer());
        }
    }

    public class MongoDbAsyncRepository<T> : IAsyncRepository<T>, IEventsSubscriber
            where T : AggregateRoot
    {
        private string connString =
                "mongodb://665b2003-0ee0-4-231-b9ee:Jwq0fxQx2aeuqqd7C3Rm4C4asScPCmIN5ivu53MhcxUVwCWoVQlvs6pny1Rd6DpVOwefPp48mlyWdoWwYL9o8g==@665b2003-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        private readonly HashSet<Guid> _entityIds = new HashSet<Guid>();

        private MongoClient _client;
        private IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public MongoDbAsyncRepository()
        {
            _client = new MongoClient(connString);
            _database = _client.GetDatabase("EvcProver");
            _collection = _database.GetCollection<T>(typeof(T).Name);

            BsonSerializer.RegisterSerializer(typeof(DeviceInstance), new DeviceInstanceBsonSerializer());
        }


        /// <inheritdoc />
        public Task<T> UpsertAsync(T entity)
        {
            if (_entityIds.Contains(entity.Id))
            {
                _collection.FindOneAndUpdateAsync(e => e.Id == entity.Id, new ObjectUpdateDefinition<T>(entity));
            }
            else
            {
                _collection.InsertOneAsync(entity);
                _entityIds.Add(entity.Id);
            }

            return Task.FromResult(entity);
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task DeleteAsync(T entity) => throw new NotImplementedException();

        /// <inheritdoc />
        public async Task<T> GetAsync(Guid id)
        {
            var result = await _collection.FindAsync(t => t.Id == id);
            return result.First();
        }

        /// <inheritdoc />
        public Task<int> CountAsync(ISpecification<T> spec) => throw new NotImplementedException();

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate = null)
        {
            predicate = predicate ?? ((x) => true);

            var cursor = await _collection.FindAsync((x) => true);
            return cursor.ToEnumerable().AsQueryable();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<T>> ListAsync()
        {
            var result = await _collection.FindAsync(t => true);
            return await result.ToListAsync();
        }

        /// <inheritdoc />
        //public IQbservable<T> QueryObservable(Expression<Func<T, bool>> predicate = null)
        //{
        //    return Observable.StartAsync(() => Query(predicate)).AsQbservable();
        //}
    }


    public class MongoDbRepository<T> : IRepository<T> where T : class
    {
        private string connString =
                "mongodb://665b2003-0ee0-4-231-b9ee:Jwq0fxQx2aeuqqd7C3Rm4C4asScPCmIN5ivu53MhcxUVwCWoVQlvs6pny1Rd6DpVOwefPp48mlyWdoWwYL9o8g==@665b2003-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;

        public MongoDbRepository()
        {
            _client = new MongoClient(connString);
            _database = _client.GetDatabase("EvcProver");
            _collection = _database.GetCollection<T>(nameof(T));
        }

        /// <inheritdoc />
        public T Add(T entity)
        {
            var collection = _database.GetCollection<T>(nameof(T));
            collection.InsertOne(entity);
            return entity;
        }

        /// <inheritdoc />
        public bool Update(T entity)
        {
            //var collection = _database.GetCollection<T>(nameof(T));

            return true;
        }

        /// <inheritdoc />
        public bool Delete(T entity) => throw new NotImplementedException();

        /// <inheritdoc />
        public IEnumerable<T> GetAll()
        {
            return _collection.FindSync(t => true).ToEnumerable();
        }

        /// <inheritdoc />
        public T Get<TId>(TId id)
        {
            return default(T);
        }
    }
}
