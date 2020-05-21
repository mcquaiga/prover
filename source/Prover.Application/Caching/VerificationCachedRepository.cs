using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Application.Specifications;
using Prover.Application.Verifications;
using Prover.Shared.Extensions;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Caching
{
	public class VerificationCachedRepository : ICachedRepository, IAsyncRepository<EvcVerificationTest>, IQueryableRepository<EvcVerificationTest>, IEntityCache<EvcVerificationTest>, IDisposable
	{
		private static readonly Expression<Func<EvcVerificationTest, bool>> _defaultPredicate = t => t.TestDateTime.IsLessThanTimeAgo(TimeSpan.FromDays(30));

		private readonly ICacheClient<EvcVerificationTest> _cache;
		private readonly ILogger<VerificationCachedRepository> _logger;
		private readonly IAsyncRepository<EvcVerificationTest> _repository;

		private bool _isInitialized;
		private IQuerySpecification<EvcVerificationTest> _cacheSpecification = VerificationQuerySpecs.Default;

		public VerificationCachedRepository(ILoggerFactory loggerFactory, IAsyncRepository<EvcVerificationTest> repository, ICacheClient<EvcVerificationTest> cache = null, IScheduler scheduler = null)
		{
			_logger = loggerFactory.CreateLogger<VerificationCachedRepository>();
			_repository = repository;
			_cache = cache ?? new EntityCache<EvcVerificationTest>(loggerFactory.CreateLogger<EntityCache<EvcVerificationTest>>(), scheduler);
		}

		/// <inheritdoc />
		public IObservableCache<EvcVerificationTest, Guid> Data => _cache.Data;

		/// <inheritdoc />
		public Task<int> CountAsync(IQuerySpecification<EvcVerificationTest> spec) => _repository.CountAsync(spec);

		/// <inheritdoc />
		public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);


		/// <inheritdoc />
		public void Dispose()
		{
			(_cache as IDisposable)?.Dispose();
		}

		/// <inheritdoc />
		public async Task<EvcVerificationTest> GetAsync(Guid id)
		{
			var cachedEntity = await _cache.GetAsync(id);

			if (cachedEntity != null)
				return cachedEntity;
			var entity = await _repository.GetAsync(id);
			await _cache.SetAsync(entity);
			return entity;
		}

		/// <inheritdoc />
		public Task<IReadOnlyList<EvcVerificationTest>> ListAsync() => Task.FromResult((IReadOnlyList<EvcVerificationTest>)_cache.Data.Items);

		/// <inheritdoc />
		public Task<IEnumerable<EvcVerificationTest>> QueryAsync(IQuerySpecification<EvcVerificationTest> specification) => _repository.QueryAsync(specification);

		/// <inheritdoc />
		public IObservable<EvcVerificationTest> QueryObservable(IQuerySpecification<EvcVerificationTest> specification)
		{
			if (specification == _cacheSpecification)
				return Data.Items.ToObservable();

			return _repository.QueryObservable(specification);
		}

		/// <inheritdoc />
		public Task Refresh(IQuerySpecification<EvcVerificationTest> specification = null)
		{
			_cacheSpecification = specification ?? _cacheSpecification;

			return Task.Run(async () =>
			{
				_logger.LogDebug("Loading verifications from repository...");
				var stopWatch = Stopwatch.StartNew();

				var loadTask = FillCacheFromRepository().ContinueWith(task =>
				{
					_logger.LogDebug($"Finished loading verifications in {stopWatch.ElapsedMilliseconds} ms");

					if (task.IsFaulted == false)
						_isInitialized = true;
					else
						_logger.LogError(task.Exception.Flatten(), "Error loading verifications.");
				});

				await loadTask;
			});

			//.LogDebug(x => $"Finished loading verifications in {stopWatch.ElapsedMilliseconds} ms", _logger)
			//.Do(x => _isInitialized = true);
		}

		public IObservable<Unit> StartAsync(CancellationToken cancellationToken) => Observable.Return(Unit.Default);


		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

		/// <inheritdoc />
		public async Task<EvcVerificationTest> UpsertAsync(EvcVerificationTest entity)
		{
			var result = await _repository.UpsertAsync(entity);
			await _cache.SetAsync(result);
			await VerificationEvents.OnSave.Publish(result);
			return result;
		}

		private IObservable<EvcVerificationTest> LoadFromRepository()
		{
			if (_repository is IQueryableRepository<EvcVerificationTest> loadObservable)
				return loadObservable.QueryObservable(_cacheSpecification);

			return Observable.FromAsync(async () =>
			{
				return await _repository.QueryAsync(_cacheSpecification)
										.ToObservable()
										.SelectMany(i => i);
			});
			//return loader;
		}

		private Task FillCacheFromRepository()
		{
			var loader = LoadFromRepository();
			return _cache.LoadAsync(loader);
		}
	}
}