using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Dashboards
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
                this.OneWayBind(ViewModel, vm => vm.Total, v => v.TotalTestsTextBlock.Text);

            });
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            return;
        }
    }
}
