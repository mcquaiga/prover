using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.Dashboard
{
    public static class DashboardFilters
    {
        public static Func<EvcVerificationTest, bool> IsVerified { get; } = test => test.Verified;
        public static Func<EvcVerificationTest, bool> IsNotVerified { get; } = test => !test.Verified;
        public static Func<EvcVerificationTest, bool> IsNotExported { get; } = test => test.ExportedDateTime == null;
    }

    public class TestsSummaryDashboardViewModel : DashboardItemViewModel, IDashboardItem, IDisposable
    {
        protected string SummaryTitle = "Summary";

        /// <inheritdoc />
        public TestsSummaryDashboardViewModel(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter = null)
            : base(groupName: "Summary", sortOrder: 0)
        {
            var cacheStream = GenerateCacheStream(entityCache, parentFilter);
            var listStream = cacheStream.AsObservableList();
            TestStream = cacheStream; 
            var countChanged = listStream.CountChanged;

            //var passed = cacheStream.Filter(DashboardFilters.IsVerified).AsObservableList();
            //var failed = cacheStream.Filter(DashboardFilters.IsNotVerified).AsObservableList();
            //var notExported = cacheStream.Filter(DashboardFilters.IsNotExported)
            //                             .AsObservableList();
            //this.WhenAnyObservable(x => x.TestStream.CountChanged());
                
            countChanged.Select(_ => listStream.Items.Count(DashboardFilters.IsVerified))
                        .ToPropertyEx(this, x => x.TotalPassed).DisposeWith(Cleanup);

            countChanged.Select(_ => listStream.Items.Count(DashboardFilters.IsNotVerified))
                        .ToPropertyEx(this, x => x.TotalFailed).DisposeWith(Cleanup);

            countChanged.Select(_ => listStream.Items.Count(DashboardFilters.IsNotExported))
                        .ToPropertyEx(this, x => x.TotalNotExported).DisposeWith(Cleanup);

            //listStream.CountChanged.ToPropertyEx(this, x => x.TotalTests).DisposeWith(Cleanup);

            this.WhenAnyValue(x => x.TotalPassed, x => x.TotalFailed, (p, f) => p + f)
                .ToPropertyEx(this, x => x.TotalTests); 
            
            this.WhenAnyValue(x => x.TotalTests)
                .Where(c => c > 0)
                .Select(total => (TotalPassed / total.ToDecimal() * 100m).ToInt32())
                .ToPropertyEx(this, x => x.PassPercentage);

            cacheStream.Avg(x => x.SubmittedDateTime?.Subtract(x.TestDateTime).TotalSeconds)
                       .Select<double, TimeSpan?>(d => TimeSpan.FromSeconds(d))
                       .ToPropertyEx(this, x => x.AverageDuration)
                       .DisposeWith(Cleanup);
        }

        [Reactive]public IObservable<IChangeSet<EvcVerificationTest>> TestStream { get; set; }


        public extern int TotalTests { [ObservableAsProperty] get; }
        public extern int TotalPassed { [ObservableAsProperty] get; }
        public extern int TotalFailed { [ObservableAsProperty] get; }

        public extern int PassPercentage { [ObservableAsProperty] get; }
        public extern int TotalNotExported { [ObservableAsProperty] get; }
        public extern TimeSpan? AverageDuration { [ObservableAsProperty] get; }
        
    }
}