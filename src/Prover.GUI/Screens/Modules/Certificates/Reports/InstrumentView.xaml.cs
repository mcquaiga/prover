using System.Windows.Controls;
using Prover.GUI.Screens.Modules.Certificates.Common;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.Certificates.Reports
{
    /// <summary>
    ///     Interaction logic for InstrumentView.xaml
    /// </summary>
    public partial class InstrumentView : UserControl, IViewFor<VerificationViewModel>
    {
        public InstrumentView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (VerificationViewModel) value;
        }

        public VerificationViewModel ViewModel { get; set; }
    }
}