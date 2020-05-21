using Devices.Core.Repository;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Prover.Application.Services;

namespace Prover.Application.Dashboard
{
	public class DashboardService
	{
		private readonly List<IDashboardItem> _dashboardItems = new List<IDashboardItem>();
		private readonly Subject<Func<EvcVerificationTest, bool>> _dashboardSharedFilter = new Subject<Func<EvcVerificationTest, bool>>();
		private readonly IDeviceRepository _deviceRepository;
		private IObservableCache<EvcVerificationTest, Guid> _cache;

		public DashboardService(IEntityCache<EvcVerificationTest> entityCache, IDeviceRepository deviceRepository)
		{
			//_entityCache = entityCache;
			_deviceRepository = deviceRepository;

			_cache = entityCache.Data.Connect()
									.Filter(_dashboardSharedFilter)
									.AsObservableCache();
		}

		public void ApplyFilter(string filterKey)
		{
			_dashboardSharedFilter.OnNext(test => VerificationFilters.TimeAgoFilters[filterKey].Invoke(test.TestDateTime));
		}

		public ICollection<string> DateFilters { get; } = VerificationFilters.TimeAgoFilters.Select(k => k.Key).ToList();

		public IEnumerable<IDashboardItem> CreateDashboard()
		{
			CreateDeviceViews(_cache);
			CreateSummaryItem(_cache);

			return _dashboardItems;
		}

		private void CreateDeviceViews(IObservableCache<EvcVerificationTest, Guid> cache)
		{
			//var filter = new Func<DeviceType, Func<EvcVerificationTest, bool>>(d => v => v.Device.DeviceType.Id == d.Id);

			_dashboardItems.AddRange(_deviceRepository
									 .GetAll().Select(deviceType => new VerifiedCountsDashboardViewModel(deviceType.Name, "By Device", cache, v => v.Device.DeviceType.Id == deviceType.Id)));
		}

		private void CreateSummaryItem(IObservableCache<EvcVerificationTest, Guid> cache)
		{
			_dashboardItems.Add(new SummaryDashboardViewModel(cache));
		}
	}
}