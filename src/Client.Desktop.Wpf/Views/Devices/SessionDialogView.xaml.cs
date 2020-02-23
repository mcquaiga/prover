using System;
using System.Reactive.Disposables;
using Client.Desktop.Wpf.ViewModels.Devices;
using ReactiveUI;

namespace Client.Wpf.Views.Devices
{
    /// <summary>
    /// Interaction logic for SessionDialogView.xaml
    /// </summary>
    public partial class SessionDialogView : ReactiveUserControl<SessionDialogViewModel>, IDisposable
    {
        public SessionDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
             
                this.OneWayBind(this.ViewModel, vm => vm.TitleText, v => v.TitleText.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StatusText, v => v.StatusText.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressTotal, v => v.StatusProgressBar.Maximum).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Progress, v => v.StatusProgressBar.Value).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton.Command).DisposeWith(d);

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
