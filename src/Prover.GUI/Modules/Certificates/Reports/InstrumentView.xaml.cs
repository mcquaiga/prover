using System.Windows.Controls;
using Prover.GUI.Modules.Certificates.Common;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Reports
{
    /// <summary>
    /// Interaction logic for InstrumentView.xaml
    /// </summary>
    public partial class InstrumentView : UserControl, IViewFor<VerificationViewModel>
    {
        public InstrumentView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (VerificationViewModel) value; }
        }

        public VerificationViewModel ViewModel { get; set; }
    }
}
