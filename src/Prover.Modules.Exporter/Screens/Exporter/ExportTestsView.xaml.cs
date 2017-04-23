using ReactiveUI;

namespace Prover.Modules.Exporter.Screens.Exporter
{
    /// <summary>
    ///     Interaction logic for CreateCertificateView.xaml
    /// </summary>
    public partial class ExportTestsView : IViewFor<ExportTestsViewModel>
    {
        public ExportTestsView()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ExportTestsViewModel) value; }
        }

        public ExportTestsViewModel ViewModel { get; set; }
    }
}