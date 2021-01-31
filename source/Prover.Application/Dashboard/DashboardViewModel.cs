using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Application.Extensions;
using Prover.Application.ViewModels;

namespace Prover.Application.Dashboard
{
	public class DashboardViewModel : ViewModelBase
	{
		private readonly DashboardService _dashboardService;
		private readonly IEntityCache<EvcVerificationTest> _cache;

		public DashboardViewModel(
				DashboardService dashboardService,
				IEntityCache<EvcVerificationTest> cache)
		{
			_dashboardService = dashboardService;
			_cache = cache;
			DateFilters = dashboardService.DateFilters;

			DashboardItems = dashboardService.CreateDashboard()
											 .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
											 .ToList();
			GroupedItems = DashboardItems
								   .GroupBy(i => i.GroupName, i => i, (group, items) => new DashboardGroup() { GroupName = group, Items = items.ToList() })
								   .ToList();

			ApplyDateFilter = ReactiveCommand.Create<string>(_dashboardService.ApplyFilter, outputScheduler: RxApp.MainThreadScheduler)
											 .DisposeWith(Cleanup);

			RefreshData = ReactiveCommand.CreateFromTask(() => _cache.Refresh())
										 .DisposeWith(Cleanup);


			RefreshData.IsExecuting
					   .Select(x => !x)
					   .ObserveOn(RxApp.MainThreadScheduler)
					   .ToPropertyEx(this, x => x.IsLoaded, scheduler: RxApp.MainThreadScheduler, initialValue: false)
					   .DisposeWith(Cleanup);

		}

		public ReactiveCommand<Unit, Unit> RefreshData { get; protected set; }

		public ReactiveCommand<string, Unit> ApplyDateFilter { get; protected set; }

		[Reactive] public string DefaultSelectedDate { get; set; } = "7d";

		public extern bool IsLoaded { [ObservableAsProperty] get; }

		public ICollection<string> DateFilters { get; }

		public ICollection<IDashboardItem> DashboardItems { get; }

		public ICollection<DashboardGroup> GroupedItems { get; }

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{

		}
	}

	public class DashboardGroup
	{
		public string GroupName { get; set; }
		public ICollection<IDashboardItem> Items { get; set; }
	}
}