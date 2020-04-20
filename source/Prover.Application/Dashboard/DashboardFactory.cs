using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Devices.Core.Repository;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Extensions;

namespace Prover.Application.Dashboard
{
    public class DashboardFactory
    {
        public readonly Dictionary<string, Func<DateTime, bool>> DateFilters = new Dictionary<string, Func<DateTime, bool>>
        {
                {"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
                {"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
                {"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
                {"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
                {"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
        };


        private readonly IDeviceRepository _deviceRepository;
        private readonly IEntityDataCache<EvcVerificationTest> _entityCache;

        private readonly Subject<Func<EvcVerificationTest, bool>> _parentFilterObservable = new Subject<Func<EvcVerificationTest, bool>>();

        public DashboardFactory(IEntityDataCache<EvcVerificationTest> entityCache, IDeviceRepository deviceRepository)
        {
            _entityCache = entityCache;
            _deviceRepository = deviceRepository;
        }

        public IEnumerable<IDashboardItem> CreateDashboard()
        {
            var items = new List<IDashboardItem>();

            items.AddRange(CreateDeviceViews(_parentFilterObservable));
            items.Add(CreateSummaryItem(_parentFilterObservable ));

            //_parentFilterObservable.Subscribe();
            return items;
        }

        private IEnumerable<IDashboardItem> CreateDeviceViews(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            //var filter = new Func<DeviceType, Func<EvcVerificationTest, bool>>(d => v => v.Device.DeviceType.Id == d.Id);

            return _deviceRepository.GetAll()
                                    .Select(d => new VerifiedCountsDashboardViewModel(
                                            _entityCache,
                                            d.Name,
                                            "By Device Type", 
                                            parentFilter,
                                            v => v.Device.DeviceType.Id == d.Id));
        }

        public void BuildGlobalFilter(string dateTimeKey)
        {
            _parentFilterObservable.OnNext(test => DateFilters[dateTimeKey].Invoke(test.TestDateTime));
        }

        private IEnumerable<IDashboardItem> CreateVerifiedViews(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            return new[]
            {
                    new ValueDashboardViewModel(_entityCache, "Total tests", "Totals", v => v.ArchivedDateTime == null, parentFilter, 1),
                    new ValueDashboardViewModel(_entityCache, "Exported tests", "Totals", v => v.ExportedDateTime != null, parentFilter, 2)
            };
        }

        private IDashboardItem CreateSummaryItem(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            return new SummaryDashboardViewModel(_entityCache, parentFilter);
        }

        private IDashboardValueViewModel GetCounterItem(string title, string groupName, Func<EvcVerificationTest, bool> predicate, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
            => new ValueDashboardViewModel(_entityCache, title, groupName, predicate, parentFilter);
    }
}