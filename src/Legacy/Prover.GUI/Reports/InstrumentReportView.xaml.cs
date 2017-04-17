using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Reports
{
    /// <summary>
    ///     Interaction logic for InstrumentReportView.xaml
    /// </summary>
    public partial class InstrumentReportView : UserControl, IViewFor<InstrumentReportViewModel>
    {
        public InstrumentReportView()
        {
            InitializeComponent();
        }

        public InstrumentReportViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (InstrumentReportViewModel) value; }
        }
    }
}