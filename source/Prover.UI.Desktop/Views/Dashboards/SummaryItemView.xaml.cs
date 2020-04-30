using ReactiveUI;
using System.Reactive.Disposables;

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
                this.OneWayBind(ViewModel, vm => vm.Totals, v => v.TotalTestsTextBlock.Text, value => value?.TotalTests.ToString()).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Totals.TotalPassed, v => v.PassedTotalTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Totals.TotalFailed, v => v.FailedTotalTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.PassPercentage, v => v.PassPercentageTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Totals.TotalNotExported, v => v.TotalNotExportedTextBlock.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.AverageDuration, v => v.AverageDurationTextBlock.Text, value => $"{value?.Minutes ?? 0}m {value?.Seconds ?? 0}s").DisposeWith(d);

            });
        }
    }
}
