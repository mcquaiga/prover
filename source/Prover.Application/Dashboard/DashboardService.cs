using Devices.Core.Repository;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Prover.Application.Dashboard
{
	public class DashboardService
	{
		private readonly List<IDashboardItem> _dashboardItems = new List<IDashboardItem>();

		private readonly Subject<Func<EvcVerificationTest, bool>> _dashboardSharedFilter = new Subject<Func<EvcVerificationTest, bool>>();
		private readonly IDeviceRepository _deviceRepository;
		private readonly IEntityDataCache<EvcVerificationTest> _entityCache;

		public readonly Dictionary<string, Func<DateTime, bool>> Filters = new Dictionary<string, Func<DateTime, bool>>
		{
				{"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
				{"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
				{"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
				{"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
				{"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
		};

		public DashboardService(IEntityDataCache<EvcVerificationTest> entityCache, IDeviceRepository deviceRepository)
		{
			_entityCache = entityCache;
			_deviceRepository = deviceRepository;
		}

		public void ApplyFilter(string filterKey)
		{
			_dashboardSharedFilter.OnNext(test => Filters[filterKey].Invoke(test.TestDateTime));
		}

		public IEnumerable<IDashboardItem> CreateDashboard()
		{
			var cache = _entityCache.Items.Connect()
									.Filter(_dashboardSharedFilter)
									.AsObservableCache();

			CreateDeviceViews(cache);
			CreateSummaryItem(cache);

			return _dashboardItems;
		}

		private void CreateDeviceViews(IObservableCache<EvcVerificationTest, Guid> cache)
		{
			//var filter = new Func<DeviceType, Func<EvcVerificationTest, bool>>(d => v => v.Device.DeviceType.Id == d.Id);

			_dashboardItems.AddRange(_deviceRepository
									 .GetAll().Select(deviceType => new VerifiedCountsDashboardViewModel(deviceType.Name, "By Device Type", cache, v => v.Device.DeviceType.Id == deviceType.Id)));
		}

		private void CreateSummaryItem(IObservableCache<EvcVerificationTest, Guid> cache)
		{
			_dashboardItems.Add(new SummaryDashboardViewModel(cache));
		}
	}
}