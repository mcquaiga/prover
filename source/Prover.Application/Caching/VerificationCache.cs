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
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Prover.Application.Caching
{
    public class VerificationCache : IEntityDataCache<EvcVerificationTest>, IDisposable
    {
        private readonly Expression<Func<EvcVerificationTest, bool>> _defaultPredicate = test => test.TestDateTime.IsLessThanTimeAgo(TimeSpan.FromDays(30));
        private readonly object _lock = new AsyncLock();
        private readonly ILogger<VerificationCache> _logger;
        private readonly Subject<Func<EvcVerificationTest, bool>> _parentFilterObservable = new Subject<Func<EvcVerificationTest, bool>>();
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;

        public readonly Dictionary<string, Func<DateTime, bool>> DateFilters = new Dictionary<string, Func<DateTime, bool>>
        {
                {"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
                {"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
                {"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
                {"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
                {"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
        };

        private CompositeDisposable _cleanup = new CompositeDisposable();
        private Func<DateTime, bool> _currentDateFilter;
        private readonly IScheduler _mainScheduler;
        private object _locker = new object();

        public VerificationCache(IAsyncRepository<EvcVerificationTest> repository, ILogger<VerificationCache> logger = null, IScheduler mainScheduler = null)
        {
            _verificationRepository = repository;
            _mainScheduler = mainScheduler ?? RxApp.TaskpoolScheduler;
            _logger = logger ?? ProverLogging.CreateLogger<VerificationCache>();

            var loader = LoadVerificationsAndMaintainCache()
                            .Filter(_parentFilterObservable.StartWith(BuildFilter("30d")))
                            .ObserveOn(_mainScheduler)
                            .Publish();

            //ApplyDateFilter("30d");

            Items = loader.AsObservableCache()
                          .DisposeWith(_cleanup);

            loader.Connect()
                  .DisposeWith(_cleanup);

            //LogChanges().DisposeWith(_cleanup);
            //LogListChanges().DisposeWith(_cleanup);
        }

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
            //var loader = GetTests(filter).Publish();
            //_cacheUpdates.PopulateFrom(loader);
        }

        private Func<EvcVerificationTest, bool> BuildFilter(string dateTimeKey)
        {
            return test => DateFilters[dateTimeKey].Invoke(test.TestDateTime);
        }

        private IObservable<IChangeSet<EvcVerificationTest, Guid>> LoadVerificationsAndMaintainCache(Func<EvcVerificationTest, bool> initialFilter = null)
        {
            return ObservableChangeSet.Create<EvcVerificationTest, Guid>(cache =>
            {
                var disposer = new CompositeDisposable();

                //var activeFilter = initialFilter ?? _defaultPredicate;
                DateTime? lastItemTestDate = null;

                VerificationEvents.OnSave.Subscribe(context =>
                {
                    cache.AddOrUpdate(context.Input);
                    //cache.Refresh(context.Input);
                });

                //var refresher = RxApp.TaskpoolScheduler.ScheduleRecurringAction(TimeSpan.FromSeconds(10), )

                IObservable<EvcVerificationTest> LoadFromRepository(Expression<Func<EvcVerificationTest, bool>> predicate)
                {
                    return Observable.Create<EvcVerificationTest>(async obs =>
                    {
                        var query = await _verificationRepository.Query(predicate);
                        var filtered = query
                                            //.Where(predicate)
                                            .Where(t => lastItemTestDate == null || t.TestDateTime > lastItemTestDate)
                                            .Take(4)
                                            .OrderBy(x => x.TestDateTime).ToList();

                        foreach (var test in filtered)
                            obs.OnNext(test);

                        lastItemTestDate = filtered.LastOrDefault()?.TestDateTime;

                        obs.OnCompleted();


                        return new CompositeDisposable();
                    });
                }

                //var random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);

                //var generator = _mainScheduler.ScheduleRecurringAction(TimeSpan.FromSeconds(5), async () =>
                //{
                //    var device = DeviceRepository.MiniMax().CreateInstance(SampleItemFiles.MiniMaxItemFile);
                //    var test = device.NewVerification();
                //    await _verificationRepository.UpsertAsync(test);
                //    await VerificationEvents.OnSave.Publish(test);
                //}).DisposeWith(disposer);

                void Load()
                {
                    cache.Clear();
                    cache.Edit(updater =>
                    {
                        LoadFromRepository(_defaultPredicate)
                                .Subscribe(updater.AddOrUpdate);
                    });
                }

                _mainScheduler.Schedule(Load);
                //cache.Edit(updater =>
                //{
                //    updater;

                //});
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

        /*
                private bool Predicate(EvcVerificationTest t) => _currentDateFilter.Invoke(t.TestDateTime);
        */
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