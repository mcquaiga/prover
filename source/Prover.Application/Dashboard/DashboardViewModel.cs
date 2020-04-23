using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Prover.Application.Dashboard
{
    public class DashboardViewModel : ReactiveObject
    {
        public DashboardViewModel(
                DashboardFactory dashboardFactory,
                IEntityDataCache<EvcVerificationTest> cache)
        {
            RefreshData = ReactiveCommand.CreateFromObservable(() =>
            {
                cache.Update();
                return Observable.Return(Unit.Default);
            });

            ApplyDateFilter = ReactiveCommand.Create<string>(cache.ApplyDateFilter, outputScheduler: RxApp.MainThreadScheduler);

            DashboardItems = dashboardFactory.CreateDashboard()
                                             .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                                             .ToList();
            GroupedItems = DashboardItems
                                   .GroupBy(i => i.GroupName, i => i, (group, items) => new DashboardGroup() { GroupName = group, Items = items.ToList() })
                                   .ToList();

            DateFilters = dashboardFactory.DateFilters.Keys;

            //RefreshData = ReactiveCommand.CreateFromObservable(() =>
            //{
            //    return Observable.Return(caches.ToObservable().ForEachAsync(c => c.LoadAsync()));
            //});
            DefaultSelectedDate = "7d";
        }

        public ReactiveCommand<Unit, Unit> RefreshData { get; set; }

        [Reactive] public string DefaultSelectedDate { get; set; }

        public ICollection<string> DateFilters { get; }

        public ICollection<IDashboardItem> DashboardItems { get; }
        public ICollection<DashboardGroup> GroupedItems { get; }

        //public ReactiveCommand<Unit, Unit> LoadCaches { get; }
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