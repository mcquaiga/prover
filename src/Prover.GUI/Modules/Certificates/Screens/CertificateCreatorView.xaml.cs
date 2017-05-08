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
                ViewModel = (CertificateCreatorViewModel)DataContext;
                d(this.WhenAnyValue(x => x.ViewModel.LoadClientsCommand)
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