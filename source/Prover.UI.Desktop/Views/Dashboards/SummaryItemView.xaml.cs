using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Dashboards
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
