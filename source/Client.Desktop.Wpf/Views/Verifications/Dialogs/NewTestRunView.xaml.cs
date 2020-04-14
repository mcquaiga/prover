using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for NewTestRunView.xaml
    /// </summary>
    public partial class NewTestRunView
    {
        public NewTestRunView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.StartTestCommand, v => v.StartTestButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CancelButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.ExitButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.LoadFromFile, v => v.LoadFromFileButton).DisposeWith(d);


                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.CommPorts.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.BaudRates, v => v.BaudRates.ItemsSource, value => value.Select(x => x.ToString())).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.TachCommPorts.ItemsSource).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TestDefinitionFilePath, v => v.FilePathTextBlock.Text).DisposeWith(d);


                this.Bind(ViewModel, vm => vm.SelectedDeviceType, v => v.DeviceTypes.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selected.TachCommPort, v => v.TachCommPorts.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selected.InstrumentCommPort, v => v.CommPorts.SelectedItem).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Selected.InstrumentBaudRate, v => v.BaudRates.SelectedItem,
                        vmp => vmp.ToString(),
                        vp => string.IsNullOrEmpty(vp?.ToString()) ? 0 : int.Parse(vp.ToString()))
                    .DisposeWith(d);
            });
        }
    }
}
