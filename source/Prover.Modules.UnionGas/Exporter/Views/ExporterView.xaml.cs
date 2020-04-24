using Prover.UI.Desktop.Extensions;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for ExporterView.xaml
    /// </summary>
    public partial class ExporterView
    {
        public ExporterView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ToolbarViewModel, v => v.VerificationsGrid.ToolbarViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Data.Count, v => v.TestCountTextBlock.Text,
                    value => value == 1 ? $"{value} test" : $"{value} tests").DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Data, v => v.VerificationsGrid.DataContext).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FromDateTime, v => v.FromDateDisplay.SelectedDate);
                this.Bind(ViewModel, vm => vm.ToDateTime, v => v.ToDateDisplay.SelectedDate);



                this.BindCommand(ViewModel, vm => vm.FilterIncludeExported, v => v.IncludeExportedCheckBox).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.FilterIncludeArchived, v => v.IncludeArchivedCheckBox).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.TestDateFilter, v => v.SearchByDateButton).DisposeWith(d);



                this.CleanUpDefaults().DisposeWith(d);

            });
        }
    }
}
