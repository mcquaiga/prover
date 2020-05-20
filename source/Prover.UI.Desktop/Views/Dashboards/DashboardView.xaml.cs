using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using Prover.Application.Dashboard;
using Prover.UI.Desktop.Converters;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Dashboards
{
	/// <summary>
	/// Interaction logic for DashboardView.xaml
	/// </summary>
	[SingleInstanceView]
	public partial class DashboardView
	{
		public ICollection<IDashboardItem> Grouping { get; set; }

		public DashboardView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.WhenAnyValue(x => x.ViewModel)
					.Where(x => x != null)
					.Do(PopulateFromViewModel)
					.Subscribe()
					.DisposeWith(d);

				this.Bind(ViewModel, vm => vm.DefaultSelectedDate, v => v.DateFiltersControl.SelectedItem).DisposeWith(d);

				//this.OneWayBind(ViewModel, vm => vm.RefreshData.IsExecuting, v => v.IsLoadingStateContent.Visibility, BooleanToVisibilityHint.None, new BooleanToVisibilityTypeConverter());
				//this.OneWayBind(ViewModel, vm => vm.RefreshData.IsExecuting, v => v.DashboardViewboxContent.Visibility, BooleanToVisibilityHint.Inverse, new BooleanToVisibilityTypeConverter());

				ViewModel.RefreshData.IsExecuting
						 .ObserveOn(RxApp.MainThreadScheduler)
						 .BindTo(this, x => x.IsLoadingStateContent.Visibility, BooleanToVisibilityHint.None, new BooleanToVisibilityTypeConverter())
						 .DisposeWith(d);

				//ViewModel.RefreshData.IsExecuting
				//		 .BindTo(this, x => x.DashboardItemsControl.Visibility, BooleanToVisibilityHint.Inverse, new BooleanToVisibilityTypeConverter())
				//		 .DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.RefreshData, v => v.RefreshDataButton).DisposeWith(d);

				this.WhenAnyValue(x => x.ViewModel.RefreshData)
					.Where(_ => !ViewModel.IsLoaded)
					.SelectMany(x => x.Execute())
					.SubscribeOnDispatcher()
					//.Select(_ => Unit.Default)
					.ObserveOnDispatcher()
					//.DelaySubscription(TimeSpan.FromMilliseconds(200))
					//.InvokeCommand(ViewModel, vm => vm.RefreshData)
					.Subscribe()
					.DisposeWith(d);

				this.WhenAnyValue(x => x.ViewModel.ApplyDateFilter)
					.SelectMany(x => x.Execute(ViewModel.DefaultSelectedDate))
					.SubscribeOnDispatcher()
					.DelaySubscription(TimeSpan.FromMilliseconds(200))
					.Subscribe()
					.DisposeWith(d);
			});

		}

		private void PopulateFromViewModel(DashboardViewModel viewModel)
		{
			var collectionView = FindResource("DashboardListBoxItems") as CollectionViewSource;
			collectionView.Source = ViewModel.DashboardItems;

			DateFiltersControl.ItemsSource = viewModel.DateFilters;

		}
	}
}
