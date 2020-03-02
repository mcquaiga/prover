using System;
using System.Reactive.Disposables;
using System.Windows.Controls;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.ViewModels.Devices;
using Client.Desktop.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Devices
{
    /// <summary>
    /// Interaction logic for SessionDialogView.xaml
    /// </summary>
    public partial class LiveReadDialogView : ReactiveUserControl<DeviceSessionManager>, IDisposable
    {
        public LiveReadDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
             
                //this.OneWayBind(this.ViewModel, vm => vm.TitleText, v => v.TitleText.Text).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.StatusText, v => v.StatusText.Text).DisposeWith(d);

                //this.OneWayBind(ViewModel, vm => vm.ProgressTotal, v => v.StatusProgressBar.Maximum).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.Progress, v => v.StatusProgressBar.Value).DisposeWith(d);

                //this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton.Command).DisposeWith(d);

                //this.WhenAnyValue(x => x.ViewModel.DialogResult)
                //    .Where(x => x.HasValue && x.Value == true)
                //    .Delay(TimeSpan.FromSeconds(3))
                //    .ObserveOn(RxApp.MainThreadScheduler)
                //    .Subscribe(x => Close())
                //    .DisposeWith(d);
            });
        }


        public void Dispose()
        {

        }
    }
}
