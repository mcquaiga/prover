using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using Client.Desktop.Wpf.ViewModels.Verifications.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Dialogs
{
    /// <summary>
    /// Interaction logic for SessionDialogView.xaml
    /// </summary>
    public partial class VolumeTestDialogView : ReactiveUserControl<VolumeTestDialogViewModel>, IDisposable
    {
        public VolumeTestDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
             
                this.OneWayBind<VolumeTestDialogViewModel, VolumeTestDialogView, string, string>(this.ViewModel, vm => vm.TitleText, v => v.TitleText.Text).DisposeWith(d);
                this.OneWayBind<VolumeTestDialogViewModel, VolumeTestDialogView, string, string>(this.ViewModel, vm => vm.StatusText, v => v.StatusText.Text).DisposeWith(d);

                this.OneWayBind<VolumeTestDialogViewModel, VolumeTestDialogView, int, double>(ViewModel, vm => vm.ProgressTotal, v => v.StatusProgressBar.Maximum).DisposeWith(d);
                this.OneWayBind<VolumeTestDialogViewModel, VolumeTestDialogView, int, double>(ViewModel, vm => vm.Progress, v => v.StatusProgressBar.Value).DisposeWith(d);

                //this.BindCommand<VolumeTestDialogView, VolumeTestDialogViewModel, ReactiveCommand<Unit, Unit>, ICommand>(ViewModel, vm => vm.CancelCommand, v => v.CancelButton.Command).DisposeWith(d);

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
