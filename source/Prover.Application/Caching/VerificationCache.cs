using Devices.Core.Interfaces;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Verifications;
using Prover.Shared.Extensions;
using Prover.Shared.Storage.Interfaces;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Prover.Application.Caching
{
	public class VerificationCache : IEntityDataCache<EvcVerificationTest>, IDisposable
	{
		private readonly IConnectableObservable<IChangeSet<EvcVerificationTest, Guid>> _loader;

		private readonly object _lock = new AsyncLock();
		private readonly ILogger<VerificationCache> _logger;
		private readonly IScheduler _mainScheduler;
		private readonly Subject<Func<EvcVerificationTest, bool>> _parentFilterObservable = new Subject<Func<EvcVerificationTest, bool>>();
		private readonly Subject<Unit> _signalUpdate = new Subject<Unit>();
		private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;

		public readonly Dictionary<string, Func<DateTime, bool>> DateFilters = new Dictionary<string, Func<DateTime, bool>>
		{
				{"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
				{"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
				{"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
				{"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
				{"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
		};

		private readonly CompositeDisposable _cleanup = new CompositeDisposable();
		private Func<DateTime, bool> _currentDateFilter;
		private object _locker = new object();

		public VerificationCache(IAsyncRepository<EvcVerificationTest> repository, ILogger<VerificationCache> logger, IScheduler mainScheduler = null)
		{
			_verificationRepository = repository;
			_mainScheduler = mainScheduler ?? RxApp.TaskpoolScheduler;
			_logger = logger ?? ProverLogging.CreateLogger<VerificationCache>();
			_loader = LoadVerificationsAndMaintainCache(BuildFilter("30d")).ObserveOn(_mainScheduler).Publish();


			Items = _loader.AsObservableCache().DisposeWith(_cleanup);
			_loader.Connect().DisposeWith(_cleanup);
			//LogChanges().DisposeWith(_cleanup);
		}



		public IObservableCache<EvcVerificationTest, Guid> Items { get; set; }

		public void ApplyDateFilter(string dateTimeKey)
		{
			_currentDateFilter = DateFilters[dateTimeKey];
			_parentFilterObservable.OnNext(BuildFilter(dateTimeKey).Compile());
		}

		public void Dispose()
		{
			_cleanup?.Dispose();
		}

		public IObservableCache<EvcVerificationTest, Guid> LoadCache(Expression<Func<EvcVerificationTest, bool>> filter) => LoadVerificationsAndMaintainCache(filter)
																															//.ToObservableChangeSet(k => k.Id, limitSizeTo: 2000)
																															.ObserveOn(_mainScheduler).Publish().AsObservableCache();

		public void Update(Expression<Func<EvcVerificationTest, bool>> filter = null)
		{
			_signalUpdate.OnNext(Unit.Default);
		}

		protected IObservable<IChangeSet<EvcVerificationTest, Guid>> LoadVerificationsAndMaintainCache(Expression<Func<EvcVerificationTest, bool>> filter)
		{
			return ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
			{
				var disposer = new CompositeDisposable();
				VerificationEvents.OnSave.Subscribe(context => { cache.AddOrUpdate(context.Input); });

				void Load()
				{
					cache.Clear();

					//cache.Edit(updater =>
					//{
					//	using (LoadFromRepository(filter)
					//			.Subscribe(updater.AddOrUpdate, updater.Refresh))
					//	{
					//	}
					//});

					LoadFromRepository(filter)
							.Subscribe(cache.AddOrUpdate, cache.Refresh);
				}

				_signalUpdate.Do(_ => Load()).Subscribe();
				_mainScheduler.Schedule(Load);
				return disposer;
			}, test => test.Id);
		}

		private Expression<Func<EvcVerificationTest, bool>> BuildFilter(string dateTimeKey)
		{
			return test => DateFilters[dateTimeKey].Invoke(test.TestDateTime);
		}


		private IObservable<EvcVerificationTest> LoadFromRepository(Expression<Func<EvcVerificationTest, bool>> predicate)
		{
			return Observable.Create<EvcVerificationTest>(async obs =>
			{
				_logger.LogDebug("Loading verifications from database...");

				var stopWatch = Stopwatch.StartNew();

				var query = await _verificationRepository.Query();

				foreach (var test in query.Where(predicate.Compile())
											  .OrderBy(x => x.TestDateTime))
				{
					obs.OnNext(test);
				}

				obs.OnCompleted();

				//var filtered = query.Where(predicate.Compile())
				//					.OrderBy(x => x.TestDateTime)
				//					.ToObservable()
				//					//.LogDebug(x => $"	Id: {x.Id}", _logger)
				//					.Subscribe(obs.OnNext, obs.OnCompleted);

				stopWatch.Stop();
				_logger.LogDebug($"Finished loading database in {stopWatch.ElapsedMilliseconds} ms.");

				return new CompositeDisposable();
			});
		}

		private IDisposable LogChanges()
		{
			const string messageTemplate = "{0} {1} {2} ({3}). Verified = {4}";

			return Items.Connect().Skip(1).WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
						.Cast(test => string.Format(messageTemplate, test.Id, test.TestDateTime, test.Device.DeviceType, test.Device.CompositionShort(), test.Verified))
						.ForEachChange(change => _logger.LogDebug(change.Current)).Subscribe();
		}
	}
}

//var updates = _cacheUpdates.AsObservableCache();

//updates.Connect()
//       .LogDebug(x => $"updates")
//       .Bind(out var items)
//       .Subscribe();
//Items = items;

//Changes = updates.AsObservableCache();

//_cachedData = _cacheUpdates.Connect().Publish();

//Data = _cachedData.AsObservableCache();
//List = Data.Connect().RemoveKey().AsObservableList();

//.Synchronize(locker)
//.Filter(_parentFilterObservable, loader.WhereReasonsAre(ChangeReason.Add).Select(_ => Unit.Default))
//.LogDebug("changed")
//.SubscribeOn(RxApp.TaskpoolScheduler)
//.ObserveOn(RxApp.TaskpoolScheduler)