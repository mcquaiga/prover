using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Devices.Core.Interfaces;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Verifications;
using Prover.Shared.Extensions;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Application.Caching
{
	//public class VerificationCache : EntityCache<EvcVerificationTest>
	//{
	//	private readonly object _lock = new AsyncLock();
	//	private readonly Subject<Func<EvcVerificationTest, bool>> _parentFilterObservable = new Subject<Func<EvcVerificationTest, bool>>();
	//	private readonly Subject<Expression<Func<EvcVerificationTest, bool>>> _signalUpdate = new Subject<Expression<Func<EvcVerificationTest, bool>>>();

	//	public readonly Dictionary<string, Func<DateTime, bool>> DateFilters = new Dictionary<string, Func<DateTime, bool>>
	//	{
	//			{"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
	//			{"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
	//			{"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
	//			{"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
	//			{"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
	//	};



	//	private Func<DateTime, bool> _currentDateFilter;
	//	private object _locker = new object();

	//	public VerificationCache(IAsyncRepository<EvcVerificationTest> repository, ILogger<VerificationCache> logger, IScheduler mainScheduler = null)
	//			: base(logger, mainScheduler)
	//	{
	//		_loader = LoadVerificationsAndMaintainCache().ObserveOn(MainScheduler).Publish();
	//	}

	//	//.AsObservableCache();



	//	protected IObservable<IChangeSet<EvcVerificationTest, Guid>> LoadVerificationsAndMaintainCache()
	//	{
	//		var changes = ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
	//		{
	//			var disposer = new CompositeDisposable();
	//			VerificationEvents.OnSave.Subscribe(context => { cache.AddOrUpdate(context.Input); });
	//			_signalUpdate.Do(f => Load(f)).Subscribe();
	//			//MainScheduler.Schedule(() => Load(filter));

	//			void Load(Expression<Func<EvcVerificationTest, bool>> predicate)
	//			{
	//				cache.Edit(updater =>
	//				{
	//					//cache.Clear();
	//					LoadFromRepository(predicate).Subscribe(updater.AddOrUpdate, cache.Refresh);
	//				});

	//				//cache.Refresh();
	//			}

	//			return disposer;
	//		}, test => test.Id);
	//		return changes;
	//	}

	//	private Expression<Func<EvcVerificationTest, bool>> BuildFilter(string dateTimeKey)
	//	{
	//		return test => DateFilters[dateTimeKey].Invoke(test.TestDateTime);
	//	}


	//	private IObservable<EvcVerificationTest> LoadFromRepository(Expression<Func<EvcVerificationTest, bool>> predicate)
	//	{
	//		return Observable.Create<EvcVerificationTest>(async obs =>
	//		{
	//			//Logger.LogDebug("Loading verifications from database...");
	//			//var stopWatch = Stopwatch.StartNew();
	//			//var query = await Repository.Query();

	//			//foreach (var test in query.Where(predicate.Compile()).OrderBy(x => x.TestDateTime))
	//			//	obs.OnNext(test);
	//			//obs.OnCompleted();
	//			//stopWatch.Stop();
	//			//Logger.LogDebug($"Finished loading database in {stopWatch.ElapsedMilliseconds} ms.");
	//			return new CompositeDisposable();
	//		});
	//	}

	//	private IDisposable LogChanges()
	//	{
	//		const string messageTemplate = "{0} {1} {2} ({3}). Verified = {4}";

	//		return Data.Connect()
	//					.Skip(1)
	//					.WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
	//					.Cast(test => string.Format(messageTemplate, test.Id, test.TestDateTime, test.Device.DeviceType, test.Device.CompositionShort(), test.Verified))
	//					.ForEachChange(change => Logger.LogDebug(change.Current))
	//					.Subscribe();
	//	}
	//}
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