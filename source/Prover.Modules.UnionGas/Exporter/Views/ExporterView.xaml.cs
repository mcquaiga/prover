using System.Reactive.Disposables;
using ReactiveUI;

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
                this.OneWayBind(ViewModel, vm => vm.TestsByJobNumberViewModel,
                    view => view.TestsByJobNumberContentControl.ViewModel).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.VisibleTests, v => v.VisibleItemsListBox.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.VisibleTests.Count, v => v.TestCountTextBlock.Text,
                    value => value == 1 ? $"{value} test" : $"{value} tests").DisposeWith(d);


                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.JobIdsList, v => v.JobIdsComboBox.ItemsSource).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IncludeExportedTests, v => v.IncludeExportedCheckBox.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.IncludeArchived, v => v.IncludeArchivedCheckBox.IsChecked).DisposeWith(d);
            });
        }
    }
}
