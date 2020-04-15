using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.Dashboard
{
    public class DashboardViewModel : ReactiveObject
    {
        public DashboardViewModel(
                DashboardFactory dashboardFactory,
                IEnumerable<ICacheManager> caches)
        {
            LoadCaches = ReactiveCommand.CreateFromObservable(() =>
            {
                caches.ForEach(c => c.Update()).ToObservable();
                return Observable.Return(Unit.Default);
            });

            ApplyDateFilter = ReactiveCommand.Create<string>(dashboardFactory.BuildGlobalFilter, outputScheduler: RxApp.MainThreadScheduler);
            
            DashboardItems = dashboardFactory.CreateDashboard()
                                             .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                                             .ToList();
            GroupedItems = DashboardItems
                                   .GroupBy(i => i.GroupName, i => i, (group, items) => new DashboardGroup(){ GroupName = group, Items = items.ToList()})
                                   .ToList();

            DateFilters = dashboardFactory.DateFilters.Keys;

            RefreshData = ReactiveCommand.CreateFromObservable(() =>
            {
                return Observable.Return(caches.ToObservable().ForEachAsync(c => c.LoadAsync()));
            });
        }

        public ReactiveCommand<Unit, Task> RefreshData { get; set; }

        [Reactive] public string DefaultSelectedDate { get; set; } = "7d";

        public ICollection<string> DateFilters { get; }

        public ICollection<IDashboardItem> DashboardItems { get; }
        public ICollection<DashboardGroup> GroupedItems { get; }

        public ReactiveCommand<Unit, Unit> LoadCaches { get; }
        public ReactiveCommand<string, Unit> ApplyDateFilter { get; }

        private Func<EvcVerificationTest, bool> BuildTestDateTimeFilter(Func<DateTime, bool> dateTime)
        {
            return test => dateTime.Invoke(test.TestDateTime);
        }
    }

    public class DashboardGroup
    {
        public string GroupName { get; set; }
        public ICollection<IDashboardItem> Items { get; set; }
    }
}