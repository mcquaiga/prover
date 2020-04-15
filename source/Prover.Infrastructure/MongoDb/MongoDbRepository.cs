using System;
using System.Collections.Generic;
using System.Text;
using Prover.Shared.Domain;
using Prover.Shared.Interfaces;
using MongoDB.Driver;
using Prover.Domain.EvcVerifications;

namespace Prover.Infrastructure.MongoDb
{
    public class MongoDbRepository<T> : IRepository<T>
        where T : AggregateRoot
    {
        private string connString =
                "mongodb://665b2003-0ee0-4-231-b9ee:Jwq0fxQx2aeuqqd7C3Rm4C4asScPCmIN5ivu53MhcxUVwCWoVQlvs6pny1Rd6DpVOwefPp48mlyWdoWwYL9o8g==@665b2003-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        private MongoClient _client;
        private IMongoDatabase _database;

        public MongoDbRepository()
        {
            _client = new MongoClient(connString);
            _database = _client.GetDatabase("EvcProver");
            _database.CreateCollection("Verifications");

        }
        
        /// <inheritdoc />
        public T Add(T entity) => throw new NotImplementedException();//_database.GetCollection<(entity);

        /// <inheritdoc />
        public bool Update(T entity) => throw new NotImplementedException();

        /// <inheritdoc />
        public bool Delete(T entity) => throw new NotImplementedException();

        /// <inheritdoc />
        public IEnumerable<T> GetAll() => throw new NotImplementedException();

        /// <inheritdoc />
        public T Get(Guid id) => throw new NotImplementedException();
    }
}
