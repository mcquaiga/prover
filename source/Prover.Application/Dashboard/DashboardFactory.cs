using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Repository;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
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

        public DashboardFactory(IEntityDataCache<EvcVerificationTest> entityCache, IDeviceRepository deviceRepository)
        {
            _entityCache = entityCache;
            _deviceRepository = deviceRepository;
        }

        public IEnumerable<IDashboardItem> CreateViews(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            return CreateDeviceViews(parentFilter)
                    .Union(CreateVerifiedViews(parentFilter));


        }

        private IEnumerable<IDashboardItem> CreateDeviceViews(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            //var filter = new Func<DeviceType, Func<EvcVerificationTest, bool>>(d => v => v.Device.DeviceType.Id == d.Id);

            return _deviceRepository.GetAll()
                                    .Select(d => new VerifiedCountsDashboardViewModel(
                                            _entityCache,
                                            d.Name,
                                            "By Device Type", parentFilter,
                                            v => v.Device.DeviceType.Id == d.Id));
        }
        

        private IEnumerable<IDashboardItem> CreateVerifiedViews(IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            return new[]
            {
                    new ValueDashboardViewModel(_entityCache, "Tested", "Totals", v => v.ArchivedDateTime == null, parentFilter, 1),
                    new ValueDashboardViewModel(_entityCache, "Exported", "Totals", v => v.ExportedDateTime != null, parentFilter, 2)
            };
        }

        private IDashboardValueViewModel GetCounterItem(string title, string groupName, Func<EvcVerificationTest, bool> predicate, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
            => new ValueDashboardViewModel(_entityCache, title, groupName, predicate, parentFilter);
    }
}