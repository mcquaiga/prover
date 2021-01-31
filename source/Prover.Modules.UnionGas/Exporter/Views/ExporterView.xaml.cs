using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prover.UI.Desktop.Extensions;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
	/// <summary>
	///     Interaction logic for ExporterView.xaml
	/// </summary>
	[SingleInstanceView]
	public partial class ExporterView
	{
		private readonly Func<int, string> CountToStringConverter = value => value == 1 ? $"{value} test" : $"{value} tests";

		public ExporterView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.WhenAnyValue(x => x.ViewModel)
					.Where(x => x != null)
					.Do(PopulateFromViewModel)
					.Subscribe()
					.DisposeWith(d);

				this.OneWayBind(ViewModel, vm => vm.Data.Count, v => v.TestCountTextBlock.Text, CountToStringConverter)
					.DisposeWith(d);

				SetupDateTimeFilters(d);

				this.BindCommand(ViewModel, vm => vm.FilterIncludeExported, v => v.IncludeExportedCheckBox).DisposeWith(d);
				this.BindCommand(ViewModel, vm => vm.FilterIncludeArchived, v => v.IncludeArchivedCheckBox).DisposeWith(d);

				this.CleanUpDefaults().DisposeWith(d);
			});
		}

		private void PopulateFromViewModel(ExporterViewModel viewModel)
		{
			DeviceTypes.ItemsSource = viewModel.DeviceTypes;
			VerificationsGrid.DataContext = viewModel.Data;
			VerificationsGrid.ToolbarViewModel = viewModel.ToolbarViewModel;

			//this.OneWayBind(ViewModel, vm => vm.ToolbarViewModel, v => v.).DisposeWith(d);
			//this.OneWayBind(ViewModel, vm => vm.Data, v => v.).DisposeWith(d);
		}

		private void SetupDateTimeFilters(CompositeDisposable d)
		{
			this.Bind(ViewModel, vm => vm.FromDateTime, v => v.FromDateDisplay.SelectedDate).DisposeWith(d);
			this.Bind(ViewModel, vm => vm.ToDateTime, v => v.ToDateDisplay.SelectedDate).DisposeWith(d);
			this.BindCommand(ViewModel, vm => vm.TestDateFilter, v => v.SearchByDateButton).DisposeWith(d);
		}
	}
}