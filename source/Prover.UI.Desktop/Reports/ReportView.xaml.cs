using ReactiveUI;
using System.Reactive.Disposables;

namespace Prover.UI.Desktop.Reports
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
