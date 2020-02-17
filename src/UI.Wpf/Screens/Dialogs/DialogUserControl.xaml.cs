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
using ReactiveUI;

namespace Client.Wpf.Screens.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogUserControl.xaml
    /// </summary>
    public partial class DialogUserControl : ReactiveDialog<DialogManager>
    {
        public DialogUserControl()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                //this.OneWayBind(ViewModel, vm => vm.DialogViewModel, v => v.DialogHost.IsOpen, value => value != null)
                ////    .DisposeWith(d);
                //    this.OneWayBind(ViewModel, vm => vm.DialogView, v => v.DialogContent.Content).DisposeWith(d);
                //    this.OneWayBind(ViewModel, vm => vm.DialogViewModel, v => v.DialogContent.DataContext).DisposeWith(d);
                });
        }
    }
}
