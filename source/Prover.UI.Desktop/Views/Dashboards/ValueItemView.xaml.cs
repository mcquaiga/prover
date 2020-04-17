using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Dashboards
{
    /// <summary>
    /// Interaction logic for TestsByDayView.xaml
    /// </summary>
    public partial class ValueItemView
    {
        public ValueItemView()
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
