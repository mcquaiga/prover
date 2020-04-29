using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prover.Application;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Storage.MongoDb
{

    public class CosmosJsonNetSerializer : CosmosSerializer
    {
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
        private readonly JsonSerializer Serializer;
        private readonly JsonSerializerSettings serializerSettings;

        public CosmosJsonNetSerializer()
                : this(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
        {
        }

        public CosmosJsonNetSerializer(
                JsonSerializerSettings serializerSettings
        )
        {
            this.serializerSettings = serializerSettings;
            this.Serializer = JsonSerializer.Create(this.serializerSettings);
        }

        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)(stream);
                }

                using (StreamReader sr = new StreamReader(stream))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                    {
                        return Serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }

        public override Stream ToStream<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(streamPayload, encoding: DefaultEncoding, bufferSize: 1024, leaveOpen: true))
            {
                using (JsonWriter writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.None;
                    Serializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }

            streamPayload.Position = 0;
            return streamPayload;
        }
    }

    public class CosmosDbAsyncRepository<T> : IAsyncRepository<T>, IEventsSubscriber
    where T : AggregateRoot
    {
        private static readonly string EndPointUrl = "https://4c9731fb-0ee0-4-231-b9ee.documents.azure.com:443/";
        private static readonly string AuthKey = "mgncs2xyrkkNnypoLptN4wtE9qvYYYHt8dX7hD6s5CkRU0moi5Sx95KyXTlbPiZVG9WdrzoLfLy5QvzZVnxfZg==";

        private static readonly string databaseId = "EvcProver";
        private static readonly string containerId = typeof(T).Name;

        //Reusable instance of ItemClient which represents the connection to a Cosmos endpoint          
        private Database database = null;
        private Container container = null;
        private CosmosClient client;
        private Task _warmupTask;
        private static ILogger<CosmosDbAsyncRepository<T>> _logger = ProverLogging.CreateLogger<CosmosDbAsyncRepository<T>>();

        private Subject<bool> _isInitialized = new Subject<bool>();

        public CosmosDbAsyncRepository()
        {
            CosmosClientOptions clientOptions = new CosmosClientOptions()
            {

                Serializer = new CosmosJsonNetSerializer()
            };


            client = new CosmosClient(EndPointUrl, AuthKey, clientOptions);
            _isInitialized.OnNext(false);

            // _warmupTask = Task.Run(() => Initialize());
        }

        /// <inheritdoc />
        public async Task<T> UpsertAsync(T entity)
        {
            await _warmupTask;

            var partitionKey = new PartitionKey((entity as EvcVerificationTest).Device.DeviceType.Id.ToString());
            T result = default;
            try
            {
                var response = await container.UpsertItemAsync<T>(entity, partitionKey);

                //var response = await container.ReadItemAsync<T>(entity.Id.ToString(), partitionKey);

                //response = await container.ReplaceItemAsync<T>(entity, entity.Id.ToString());

                _logger.LogDebug("Upserted item with id: {0}", response.Resource.Id);

                result = response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var response = await container.UpsertItemAsync<T>(entity, partitionKey);
                _logger.LogDebug("Created item with id: {0}", response.Resource.Id);

                result = response.Resource;
            }

            return result;
        }

        /// <inheritdoc />
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task DeleteAsync(T entity) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<T> GetAsync(Guid id) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<int> CountAsync(ISpecification<T> spec) => throw new NotImplementedException();

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate = null)
        {
            await _warmupTask.ConfigureAwait(false);

            var sqlQueryText = "SELECT * FROM c";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            var queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = this.container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true);

            return await Task.FromResult(queryResultSetIterator.AsEnumerable());

        }


        /// <inheritdoc />
        public Task<IReadOnlyList<T>> ListAsync() => throw new NotImplementedException();


        public Task Initialize()
        {




            _warmupTask = Task.Run(async () =>
            {
                database = await client.CreateDatabaseIfNotExistsAsync(databaseId);

                // Delete the existing container to prevent create item conflicts
                //using (await repo.database.GetContainer(containerId).DeleteContainerStreamAsync())
                //{ }

                // We create a partitioned collection here which needs a partition key. Partitioned collections
                // can be created with very high values of provisioned throughput (up to Throughput = 250,000)
                // and used to store up to 250 GB of data. You can also skip specifying a partition key to create
                // single partition collections that store up to 10 GB of data.
                // For this demo, we create a collection to store SalesOrders. We set the partition key to the account
                // number so that we can retrieve all sales orders for an account efficiently from a single partition,
                // and perform transactions across multiple sales order for a single account number. 
                ContainerProperties containerProperties = new ContainerProperties(containerId, partitionKeyPath: "/Device/DeviceTypeId");

                // Create with a throughput of 1000 RU/s
                container = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                throughput: 1000);
                _isInitialized.OnNext(true);
            });

            return Task.CompletedTask;
        }
    }
}
