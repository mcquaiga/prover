using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Client.Desktop.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestManagerView : ReactiveUserControl<TestManagerViewModel>
    {
        public TestManagerView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    //this.OneWayBind(ViewModel, vm => vm.TestManagerScreenState, v => v.TestStartContent.Visibility, 
                    //    value => value == TestManagerViewModel.TestManagerState.Start ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

                    //this.OneWayBind(ViewModel, vm => vm.TestManagerScreenState, v => v.TestInProgressContent.Visibility, 
                    //    value => value == TestManagerViewModel.TestManagerState.InProgress ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

                    this.WhenAnyValue(x => x.ViewModel.TestManagerScreenState)
                        .Where(s => s == TestManagerViewModel.TestManagerState.InProgress)
                        .Do(t => SetupInProgress(d));

                    SetupNewView(d);
                });
        }

        private void SetupInProgress(CompositeDisposable d)
        {
            this.OneWayBind(ViewModel, vm => vm.TestManager.TestViewModel.Tests, v => v.TestPointItems.ItemsSource).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.TestManager.TestViewModel, v => v.SiteInfoContent.ViewModel).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.TestManager.TestViewModel.VolumeTest, v => v.VolumeContentHost.ViewModel).DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
        }

        private void SetupNewView(CompositeDisposable d)
        {
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
        }
    }
}
