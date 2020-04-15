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
    public partial class SummaryCardView
    {
        public SummaryCardView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
               // this.OneWayBind(ViewModel, vm => vm.Title, v => v.TitleTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TotalTests, v => v.TotalTestsTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TotalPassed, v => v.PassedTotalTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TotalFailed, v => v.FailedTotalTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.PassPercentage, v => v.PassPercentageTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.TotalNotExported, v => v.TotalNotExportedTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AverageDuration, v => v.AverageDurationTextBlock.Text, value => $"{value?.Minutes ?? 0}m {value?.Seconds ?? 0}s").DisposeWith(d);

            });
        }
    }
}
