using System;
using System.Reactive.Linq;
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

            this.WhenActivated(d =>
            {
                d(ViewModel = (CertificateCreatorViewModel)DataContext);

                d(this.BindCommand(
                    ViewModel,
                    x => x.PrintExistingCertificateCommand,
                    x => x.PrintExistingCertificateButton,
                    x => x.SelectedExistingClientCertificate));

                d(this.WhenAnyValue(x => x.ViewModel.LoadClientsCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());

                d(this.WhenAnyValue(x => x.ViewModel.FetchNextCertificateNumberCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CertificateCreatorViewModel) value; }
        }

        public CertificateCreatorViewModel ViewModel { get; set; }
    }
}