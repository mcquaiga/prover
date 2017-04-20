using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Screens
{
    /// <summary>
    ///     Interaction logic for CreateCertificateView.xaml
    /// </summary>
    public partial class CertificateCreatorView : IViewFor<CertificateCreatorViewModel>
    {
        public CertificateCreatorView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CertificateCreatorViewModel) value; }
        }

        public CertificateCreatorViewModel ViewModel { get; set; }
    }
}