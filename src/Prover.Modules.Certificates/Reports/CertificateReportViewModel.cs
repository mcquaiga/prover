using Autofac;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.ViewModels.InstrumentsList;
using Prover.Modules.CertificatesUi.Screens.Certificates;

namespace Prover.Modules.CertificatesUi.Reports
{
    public class CertificateReportViewModel : ReactiveScreen
    {
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly IProverStore<Certificate> _certificateStore;

        public CertificateReportViewModel(IProverStore<Instrument> instrumentStore, IProverStore<Certificate> certificateStore )
        {
            _instrumentStore = instrumentStore;
            _certificateStore = certificateStore;
            LoadCertificate(certificate);
        }

        public Certificate Certificate { get; set; }

        public string CertificateDate
        {
            get { return Certificate.CreatedDateTime.ToShortDateString(); }
        }

        public InstrumentsListViewModel InstrumentsView { get; set; }

        public void LoadCertificate(Certificate certificate)
        {
            InstrumentsView = new InstrumentsListViewModel(_instrumentStore);
            InstrumentsView.GetInstrumentsByCertificateId(certificate.Id);
            Certificate = certificate;
            NotifyOfPropertyChange(() => Certificate);
        }

        private void SaveCertificate()
        {
        }
    }
}