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

namespace Client.Desktop.Wpf.Views.Dashboards
{
    /// <summary>
    /// Interaction logic for TestsByDayView.xaml
    /// </summary>
    public partial class VerifiedDashboardView
    {
        public VerifiedDashboardView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Passed, v => v.PassCountTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Failed, v => v.FailCountTextBlock.Text).DisposeWith(d);

            });
        }
    }
}
