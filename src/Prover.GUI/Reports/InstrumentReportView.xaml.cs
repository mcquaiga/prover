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

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (InstrumentReportViewModel) value;
        }

        public InstrumentReportViewModel ViewModel { get; set; }
    }
}