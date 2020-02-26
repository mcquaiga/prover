using System.Reactive.Disposables;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.ViewModels.Dialogs;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for StatusDialog.xaml
    /// </summary>
    public partial class BackgroundWorkDialog : ReactiveDialog<BackgroundWorkDialogViewModel>
    {
        public BackgroundWorkDialog()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
             
                this.OneWayBind(this.ViewModel, vm => vm.TitleText, v => v.TitleText.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StatusText, v => v.StatusText.Text).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressTotal, v => v.StatusProgressBar.Maximum).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Progress, v => v.StatusProgressBar.Value).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton.Command).DisposeWith(d);

                //this.WhenAnyValue(x => x.ViewModel.TaskCommand)
                //    .SelectMany(x => x.Execute())
                //    .Subscribe()
                //    .DisposeWith(d);
            });

            

        }

    }
}
