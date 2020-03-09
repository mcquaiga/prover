using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Desktop.Wpf.ViewModels.Devices;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Devices
{
    /// <summary>
    /// Interaction logic for SessionStatusDialogView.xaml
    /// </summary>
    public partial class SessionStatusDialogView : ReactiveUserControl<SessionStatusDialogViewModel>
    {
        public SessionStatusDialogView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {

                this.OneWayBind(ViewModel, vm => vm.TitleText, v => v.TitleText.Text)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.StatusText, v => v.StatusText.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressTotal, v => v.StatusProgressBar.Maximum).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Progress, v => v.StatusProgressBar.Value).DisposeWith(d);
            });
        }
    }
}
