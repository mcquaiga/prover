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

namespace Client.Desktop.Wpf.Reports
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView
    {
        public ReportView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.ContentViewModel, v => v.ReportHostContentControl.ViewModel).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CloseView, view => view.CloseReport).DisposeWith(d);
            });
        }
    }
}
