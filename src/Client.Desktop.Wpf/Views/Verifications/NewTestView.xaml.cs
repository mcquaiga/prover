using System.Linq;
using System.Reactive.Disposables;
using Client.Desktop.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for VerificationTest.xaml
    /// </summary>
    public partial class NewTestView : ReactiveUserControl<NewTestViewModel>
    {
        public NewTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                DataContext = ViewModel;

                this.BindCommand(ViewModel, vm => vm.StartTestCommand, v => v.StartTestButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedDeviceType, v => v.DeviceTypes.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.CommPorts.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedCommPort, v => v.CommPorts.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.BaudRates, v => v.BaudRates.ItemsSource, value => value.Select(x => x.ToString())).DisposeWith(d);
                this.Bind(ViewModel, 
                    vm => vm.SelectedBaudRate, 
                    v => v.BaudRates.SelectedItem, 
                    vmp => vmp.ToString(), 
                    vp => string.IsNullOrEmpty(vp?.ToString()) ? 0 : int.Parse(vp.ToString())
                ).DisposeWith(d);

                //Tach Settings
                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.TachCommPorts.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedTachCommPort, v => v.TachCommPorts.SelectedItem).DisposeWith(d);

            });
        }
    }
}
