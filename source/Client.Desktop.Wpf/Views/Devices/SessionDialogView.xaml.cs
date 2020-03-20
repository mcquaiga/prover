using System;
using System.Reactive.Disposables;
using Client.Desktop.Wpf.Interactions;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Devices
{
    /// <summary>
    ///     Interaction logic for SessionDialogView.xaml
    /// </summary>
    public partial class SessionDialogView : ReactiveUserControl<DeviceSessionDialogManager>, IDisposable
    {
        public SessionDialogView()
        {
            InitializeComponent();


            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, x => x.RequestCancellation, v => v.CancelButton).DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.SessionDialogContent,
                        v => v.SessionDialogContentControl.Content)
                    .DisposeWith(d);
            });
        }
        
        public void Dispose()
        {
        }
    }
}