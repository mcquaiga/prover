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
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Prover.Application.Caching
{
	public class VerificationCache : IEntityDataCache<EvcVerificationTest>, IDisposable
	{
		public readonly Dictionary<string, Func<DateTime, bool>> DateFilters = new Dictionary<string, Func<DateTime, bool>>
		{
				{"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
				{"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
				{"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
				{"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
				{"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
		};

		private readonly Expression<Func<EvcVerificationTest, bool>> _defaultPredicate = test => test.TestDateTime.IsLessThanTimeAgo(TimeSpan.FromDays(30));
		private readonly object _lock = new AsyncLock();
		private readonly ILogger<VerificationCache> _logger;
		private readonly IConnectableObservable<IChangeSet<EvcVerificationTest, Guid>> _loader;
		private readonly Subject<Func<EvcVerificationTest, bool>> _parentFilterObservable = new Subject<Func<EvcVerificationTest, bool>>();
		private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;
		private readonly IScheduler _mainScheduler;
		private CompositeDisposable _cleanup = new CompositeDisposable();
		private Func<DateTime, bool> _currentDateFilter;
		private object _locker = new object();
		private readonly Subject<Unit> _signalUpdate = new Subject<Unit>();

		public VerificationCache(IAsyncRepository<EvcVerificationTest> repository, ILogger<VerificationCache> logger = null, IScheduler mainScheduler = null)
		{
			_verificationRepository = repository;
			_mainScheduler = mainScheduler ?? RxApp.TaskpoolScheduler;
			_logger = logger ?? ProverLogging.CreateLogger<VerificationCache>();

			_loader = LoadVerificationsAndMaintainCache().ObserveOn(_mainScheduler)
														 .Publish();

			//ApplyDateFilter("30d");

			Items = _loader.AsObservableCache()
						  .DisposeWith(_cleanup);

			_loader.Connect()
				  .DisposeWith(_cleanup);

			//LogChanges().DisposeWith(_cleanup);
			//LogListChanges().DisposeWith(_cleanup);
		}

		//public IObservableCache<EvcVerificationTest, Guid> GetVerifications(IObservable<Func<EvcVerificationTest, bool>> filter)
		//{
		//	return LoadVerificationsAndMaintainCache(filterObservable: filter);
		//}

		public IObservableCache<EvcVerificationTest, Guid> Items { get; set; }
		public IObservableList<EvcVerificationTest> Data { get; set; }

		public void ApplyDateFilter(string dateTimeKey)
		{
			_currentDateFilter = DateFilters[dateTimeKey];
			_parentFilterObservable.OnNext(BuildFilter(dateTimeKey));
		}

		public void Dispose()
		{
			_cleanup?.Dispose();
		}

		/// <inheritdoc />
		public Task LoadAsync() => Task.CompletedTask;

		public void Update(Expression<Func<EvcVerificationTest, bool>> filter = null)
		{
			_signalUpdate.OnNext(Unit.Default);
		}

		private Func<EvcVerificationTest, bool> BuildFilter(string dateTimeKey)
		{
			return test => DateFilters[dateTimeKey].Invoke(test.TestDateTime);
		}

		private IObservable<IChangeSet<EvcVerificationTest, Guid>> LoadVerificationsAndMaintainCache(Func<EvcVerificationTest, bool> initialFilter = null, IObservable<Func<EvcVerificationTest, bool>> filterObservable = null)
		{
			return ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
			{
				var disposer = new CompositeDisposable();

				VerificationEvents.OnSave.Subscribe(context =>
				{
					cache.AddOrUpdate(context.Input);
				});

				void Load()
				{
					cache.Clear();
					cache.Edit(updater =>
					{
						using (LoadFromRepository(_defaultPredicate)
											.Subscribe(updater.AddOrUpdate, () => updater.Refresh()))
						{ }

					});
				}

				_signalUpdate.Do(_ => cache.Refresh())
							 .Subscribe();

				_mainScheduler.Schedule(Load);

				return disposer;
			}, test => test.Id);
		}

		private IDisposable LogChanges()
		{
			const string messageTemplate = "{0} {1} {2} ({3}). Verified = {4}";

			return Items.Connect().Skip(1).WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
					  .Cast(test => string.Format(messageTemplate, test.Id, test.TestDateTime, test.Device.DeviceType, test.Device.CompositionShort(), test.Verified))
					  .ForEachChange(change => _logger.LogDebug(change.Current)).Subscribe();
		}

		private IDisposable LogListChanges()
		{
			return Data.Connect().LogDebug(x => $"Total Adds = {x.Adds}").Subscribe();
		}

		private IObservable<EvcVerificationTest> LoadFromRepository(Expression<Func<EvcVerificationTest, bool>> predicate)
		{
			return Observable.Create<EvcVerificationTest>(async obs =>
			{
				var query = await _verificationRepository.Query();

				var filtered = query.Where(predicate.Compile())
									//.Where(t => lastItemTestDate == null || t.TestDateTime > lastItemTestDate)
									.OrderBy(x => x.TestDateTime);

				filtered.ToObservable().Subscribe(obs.OnNext, obs.OnCompleted);
				//lastItemTestDate = filtered.LastOrDefault()?.TestDateTime;

				//obs.OnCompleted();

				return new CompositeDisposable();
			});
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