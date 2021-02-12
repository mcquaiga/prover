using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;
using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Prover.Application.Caching {
	public class EntityCache<T> : ICacheClient<T>, IEntityCache<T>, IDisposable where T : AggregateRoot {
		protected readonly SourceCache<T, Guid> CachedData = new SourceCache<T, Guid>(root => root.Id);
		protected readonly CompositeDisposable Cleanup = new CompositeDisposable();
		protected IConnectableObservable<IChangeSet<T, Guid>> _loader;
		protected ILogger<EntityCache<T>> Logger;

		protected IScheduler Scheduler;


		public EntityCache(ILogger<EntityCache<T>> logger = null, IScheduler scheduler = null) {
			Logger = logger ?? ProverLogging.CreateLogger<EntityCache<T>>();

			Scheduler = scheduler ?? RxApp.TaskpoolScheduler;

			var loader = CachedData.Connect().Publish();
			Data = loader.AsObservableCache();
			loader.Connect().DisposeWith(Cleanup);
		}

		public IObservableCache<T, Guid> Data { get; set; }

		public void Dispose() {
			Dispose(true);
		}

		/// <inheritdoc />
		public Task<T> GetAsync(Guid id) => Task.FromResult(Data.Lookup(id).Value ?? default);

		public virtual Task LoadAsync(IObservable<T> entityObservable) {
			var disposer = new CompositeDisposable();

			void LoadCache() {
				CachedData.Edit(updater => {
					updater.Load(entityObservable.ToEnumerable());
					//entityObservable.LogErrors(Logger)
					//				.Subscribe(updater.AddOrUpdate, () => updater.Refresh())
					//				.DisposeWith(disposer);
				});
			}

			LoadCache();

			//}).ContinueWith(t => disposer?.Dispose());
			return Task.CompletedTask;
		}

		public Task Refresh(IQuerySpecification<EvcVerificationTest> specification = null) {
			//_signalUpdate.OnNext(specification);
			return Task.CompletedTask;
		}

		public Task SetAsync(T entity) {
			CachedData.AddOrUpdate(entity);
			return Task.CompletedTask;
		}

		protected virtual void Dispose(bool disposing) {
			Data.DisposeWith(Cleanup);
			CachedData.DisposeWith(Cleanup);
			Cleanup?.Dispose();
		}

		protected virtual IDisposable LogChanges() {
			const string messageTemplate = "Id: {0} - {1}";

			return Data.Connect()
					   .Skip(1)
					   .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
					   .Cast(item => string.Format(messageTemplate, item.Id, item))
					   .ForEachChange(change => Logger.LogDebug(change.Current))
					   .Subscribe();
		}

		//protected virtual SourceCache<T, Guid> SetupAndMaintainCache()
		//{
		//	var changes = ObservableChangeSet.Create<T, Guid>(cache =>
		//	{
		//		var disposer = new CompositeDisposable();

		//		//_signalUpdate.Do(filter => Load(filter))
		//		//			 .Subscribe()
		//		//			 .DisposeWith(disposer);

		//		//void Load(Expression<Func<T, bool>> predicate)
		//		//{
		//		//	cache.Edit(updater =>
		//		//	{
		//		//		//cache.Clear();
		//		//		LoadFromRepository(predicate).Subscribe(updater.AddOrUpdate, cache.Refresh);
		//		//	});

		//		//	//cache.Refresh();
		//		//}

		//		return disposer;
		//	}, test => test.Id);
		//	return (SourceCache<T, Guid>)changes.AsObservableCache();
		//}

		//protected virtual IObservable<T> LoadFromRepository(Expression<Func<T, bool>> predicate)
		//{
		//	return Observable.Create<T>(async obs =>
		//	{
		//		var stopWatch = Stopwatch.StartNew();

		//		Logger.LogDebug("Loading verifications from repository...");

		//		var query = await Repository.Query(predicate);

		//		foreach (var test in query.Where(predicate.Compile()))
		//			obs.OnNext(test);

		//		obs.OnCompleted();

		//		stopWatch.Stop();

		//		Logger.LogDebug($"Finished loading repository in {stopWatch.ElapsedMilliseconds} ms.");
		//		return new CompositeDisposable();
		//	});
		//}
	}
}