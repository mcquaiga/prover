using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Prover.Application;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;
using Prover.Application.Services;
using System.Diagnostics;
using DynamicData;

namespace Prover.Storage.MongoDb
{
	public class CosmosDbAsyncRepository<T> : IAsyncRepository<T>, IQueryableRepository<T>, IEventsSubscriber, IRequireInitialization
		where T : AggregateRoot
	{
		private static readonly string EndPointUrl = "https://4c9731fb-0ee0-4-231-b9ee.documents.azure.com:443/";
		private static readonly string AuthKey = "mgncs2xyrkkNnypoLptN4wtE9qvYYYHt8dX7hD6s5CkRU0moi5Sx95KyXTlbPiZVG9WdrzoLfLy5QvzZVnxfZg==";

		private static readonly string databaseId = "EvcProver";
		private static readonly string containerId = typeof(T).Name;

		//Reusable instance of ItemClient which represents the connection to a Cosmos endpoint          
		private Database _database = null;
		private Container _container = null;
		private CosmosClient _client;
		private Task _warmupTask;
		private static ILogger<CosmosDbAsyncRepository<T>> _logger = ProverLogging.CreateLogger<CosmosDbAsyncRepository<T>>();

		private Subject<bool> _isInitialized = new Subject<bool>();
		//private IObservable<Unit> _initializedObservable;
		private bool _initialized = false;

		public CosmosDbAsyncRepository()
		{
			CosmosClientOptions clientOptions = new CosmosClientOptions()
			{
				Serializer = new CosmosJsonNetSerializer()
			};

			_client = new CosmosClient(EndPointUrl, AuthKey, clientOptions);

			_isInitialized.OnNext(false);

			_warmupTask = Initialize();

			//_initializedObservable.Select(_ => true)
			//					   .Subscribe(_isInitialized);
			// _warmupTask = Task.Run(() => Initialize());
		}

		public IObservable<Unit> Initialized => _isInitialized.Where(i => i).Select(_ => Unit.Default);
		/// <inheritdoc />
		public async Task<T> UpsertAsync(T entity)
		{
			await WaitForInit();

			var partitionKey = new PartitionKey((entity as EvcVerificationTest).Device.DeviceType.Id.ToString());
			T result = default;
			try
			{
				var response = await _container.UpsertItemAsync<T>(entity, partitionKey);

				//var response = await container.ReadItemAsync<T>(entity.Id.ToString(), partitionKey);

				//response = await container.ReplaceItemAsync<T>(entity, entity.Id.ToString());

				_logger.LogDebug("Upserted item with id: {0}", response.Resource.Id);

				result = response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				var response = await _container.UpsertItemAsync<T>(entity, partitionKey);
				_logger.LogDebug("Created item with id: {0}", response.Resource.Id);

				result = response.Resource;
			}

			return result;
		}

		/// <inheritdoc />
		public Task<bool> DeleteAsync(Guid id) => throw new NotImplementedException();

		/// <inheritdoc />
		public Task DeleteAsync(T entity) => throw new NotImplementedException();

		/// <inheritdoc />
		public Task<T> GetAsync(Guid id) => throw new NotImplementedException();

		/// <inheritdoc />
		public Task<int> CountAsync(IQuerySpecification<T> spec) => throw new NotImplementedException();

		/// <inheritdoc />
		public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate = null)
		{
			await WaitForInit();

			var sqlQueryText = "SELECT * FROM c";

			_logger.LogDebug("Running query: {0}\n", sqlQueryText);

			var queryDefinition = new QueryDefinition(sqlQueryText);
			var queryResultSetIterator = _container
											 .GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true)
											 .Where(predicate);

			return queryResultSetIterator.AsEnumerable();

		}

		/// <inheritdoc />
		public async Task<IEnumerable<T>> QueryAsync(IQuerySpecification<T> specification)
		{
			await WaitForInit();
			var query = ApplySpecification(specification);
			return query.AsEnumerable();
		}

		/// <inheritdoc />
		//public IQbservable<T> QueryObservable() => throw new NotImplementedException();

		public IObservable<T> QueryObservable(IQuerySpecification<T> specification)
		{
			return Observable.Create<T>(async obs =>
			{
				var disposer = new CompositeDisposable();

				void GetItems()
				{
					var query = ApplySpecification(specification);
					//specification.CompileSpecification();

					//var query = _container
					//            .GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true)
					//            .Where(specification.Predicate);

					query.Subscribe(obs)
						 .DisposeWith(disposer);
				}

				async Task Feeder()
				{
					var queryIterator = ApplySpecification(specification).ToFeedIterator();

					var reader = Observable.FromAsync(async () =>
										   {
											   var feed = await queryIterator.ReadNextAsync();
											   return feed.ToObservable();
										   })
										   .RepeatWhen(x => Observable.Create<bool>(handler =>
																	  {
																		  if (queryIterator.HasMoreResults)
																			  handler.OnNext(true);
																		  else
																			  handler.OnCompleted();
																		  return Disposable.Empty;
																	  }))
										   .Concat()
										   .Subscribe(obs)
										   .DisposeWith(disposer);
				}


				await WaitForInit();

				GetItems();
				//await Feeder();

				return disposer;
			});
			//

			//return query.AsEnumerable();
		}

		/// <inheritdoc />
		public Task<IReadOnlyList<T>> ListAsync() => throw new NotImplementedException();

		private IQueryable<T> ApplySpecification(IQueryable<T> queryable, IQuerySpecification<T> spec)
		{
			return SpecificationEvaluator<T>.GetQuery(queryable, spec);
		}

		private IQueryable<T> ApplySpecification(IQuerySpecification<T> spec)
		{
			return ApplySpecification(_container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true), spec);
		}

		private Task WaitForInit()
		{
			return Task.WhenAll(new[] { _warmupTask });
		}

		public Task Initialize()
		{
			if (_initialized)
				return Task.CompletedTask;

			return Task.Run(async () =>
			{
				_database = await _client.CreateDatabaseIfNotExistsAsync(databaseId);

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
				_container = await _database.CreateContainerIfNotExistsAsync(
					containerProperties,
					throughput: 1000);
				_isInitialized.OnNext(true);
			});

			//return _warmupTask;
		}



	}
}
