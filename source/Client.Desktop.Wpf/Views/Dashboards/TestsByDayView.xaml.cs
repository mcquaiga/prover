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
    public partial class TestsByDayView
    {
        public TestsByDayView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Value, v => v.ValueTextBlock.Text).DisposeWith(d);

            });
        }
    }
}
