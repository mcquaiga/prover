using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    /// <summary>
    ///     Interaction logic for CreateCertificateView.xaml
    /// </summary>
    public partial class CertificateView : IViewFor<CertificateViewModel>
    {
        public CertificateView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CertificateViewModel) value; }
        }

        public CertificateViewModel ViewModel { get; set; }
    }
}