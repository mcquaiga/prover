using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for ExporterView.xaml
    /// </summary>
    public partial class ExporterView : ReactiveUserControl<ExporterViewModel>
    {
        public ExporterView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.VisibleTests, v => v.VisibleItemsListBox.ItemsSource).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource)
                    .DisposeWith(d);
               
            });
        }
    }
}
