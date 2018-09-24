using System.Windows.Controls;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.Certificates.Reports
{
    /// <summary>
    ///     Interaction logic for CertificateView.xaml
    /// </summary>
    public partial class CertificateReportView : UserControl, IViewFor<CertificateReportViewModel>
    {
        public CertificateReportView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CertificateReportViewModel) value;
        }

        public CertificateReportViewModel ViewModel { get; set; }
    }
}