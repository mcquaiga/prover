﻿using DynamicData;
using DynamicData.Aggregation;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
// ReSharper disable PossibleInvalidOperationException

namespace Prover.Application.Dashboard
{
    public class SummaryDashboardViewModel : DashboardItemViewModel, IDashboardItem, IDisposable
    {
        protected string SummaryTitle = "Summary";
        private readonly IScheduler _scheduler;
        /// <inheritdoc />
        public SummaryDashboardViewModel(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter = null, IScheduler mainScheduler = null) : base(groupName: "Summary", sortOrder: 0)
        {
            if (entityCache == null) throw new NullReferenceException(nameof(IEntityDataCache<EvcVerificationTest>));

            _scheduler = mainScheduler ?? RxApp.MainThreadScheduler;

            var shared = entityCache.Items.Connect()
                    .ObserveOn(_scheduler);

            shared.QueryWhenChanged(query => CreateSummaryTotals(query.Items.ToList()))
                    .ToPropertyEx(this, x => x.Totals, scheduler: _scheduler, initialValue: new SummaryTotals())
                    .DisposeWith(Cleanup);

            shared.Filter(t => t.SubmittedDateTime.HasValue)
                  //.SelectMany(x => x.Items.Where(t => t.SubmittedDateTime.HasValue))
                  .Avg(x => x.SubmittedDateTime.Value.Subtract(x.TestDateTime).TotalSeconds)
                  .Select<double, TimeSpan?>(seconds => TimeSpan.FromSeconds(seconds))
                  .LogDebug(x => $"totals changed {x}")
                  .ToPropertyEx(this, x => x.AverageDuration, scheduler: _scheduler)
                  .DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.Totals)
                .Where(c => c?.TotalTests > 0)
                .Select(summary => (summary.TotalPassed / summary.TotalTests.ToDecimal() * 100m).ToInt32())
                .ToPropertyEx(this, x => x.PassPercentage, scheduler: _scheduler)
                .DisposeWith(Cleanup);



        }

        private SummaryTotals CreateSummaryTotals(ICollection<EvcVerificationTest> itemCollection) => new SummaryTotals { TotalTests = itemCollection.Count, TotalPassed = itemCollection.Count(VerificationFilters.IsVerified), TotalFailed = itemCollection.Count(VerificationFilters.IsNotVerified), TotalNotExported = itemCollection.Where(VerificationFilters.IsVerified).Count(VerificationFilters.IsNotExported) };



        //[Reactive] public SummaryTotals Totals { get; set; }
        public extern SummaryTotals Totals { [ObservableAsProperty] get; }
        public extern int PassPercentage { [ObservableAsProperty] get; }
        public extern TimeSpan? AverageDuration { [ObservableAsProperty] get; }

        #region Nested type: SummaryTotals

        public class SummaryTotals
        {
            [Reactive] public int TotalTests { get; set; }
            [Reactive] public int TotalPassed { get; set; }
            [Reactive] public int TotalFailed { get; set; }
            [Reactive] public int TotalNotExported { get; set; }
        }

        #endregion
    }
}

/*
       //_cacheStream.CountChanged()
       //           .Select(_ => Items.Count(DashboardFilters.IsNotVerified))
       //           .ToPropertyEx(this, x => x.TotalFailed, scheduler: RxApp.MainThreadScheduler).DisposeWith(Cleanup);

       //_cacheStream.CountChanged()
       //           .Select(_ => Items.Count(DashboardFilters.IsNotExported))
       //           .ToPropertyEx(this, x => x.TotalNotExported, scheduler: RxApp.MainThreadScheduler).DisposeWith(Cleanup);


       //this.WhenAnyValue(x => x.TotalPassed, x => x.TotalFailed, (p, f) => p + f)
       //    .ToPropertyEx(this, x => x.TotalTests, scheduler: RxApp.MainThreadScheduler).DisposeWith(Cleanup); 



       //_cacheStream.Bind(out _items)
       //            .ObserveOn(RxApp.MainThreadScheduler)
       //            .Subscribe();

       //Items.ToObservable()
       //     .Count()
       //     .LogDebug(x => $"Count changed = {x}")
       //     .SubscribeOn(RxApp.MainThreadScheduler)
       //     .Subscribe();

       //Cleanup.Add(shared.Subscribe());
       //Cleanup.Add(_cacheStream.Subscribe());
       ////Cleanup.Add(verifiedCount.Subscribe());
       //Cleanup.Add(binder);*/
