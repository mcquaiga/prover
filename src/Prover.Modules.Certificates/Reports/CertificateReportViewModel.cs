using Autofac;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Certificates;

namespace Prover.GUI.Reports
{
    public class CertificateReportViewModel : ReactiveScreen
    {
        private readonly IContainer _container;

        public CertificateReportViewModel(IContainer container, Certificate certificate)
        {
            _container = container;

            //LoadCertificate(certificate);
        }

        public Certificate Certificate { get; set; }

        public string CertificateDate
        {
            get { return Certificate.CreatedDateTime.ToShortDateString(); }
        }

        //public InstrumentsListViewModel InstrumentsView { get; set; }

        //private void LoadCertificate(Certificate certificate)
        //{
        //    InstrumentsView = new InstrumentsListViewModel(_container);
        //    InstrumentsView.GetInstrumentsByCertificateId(certificate.Id);
        //    Certificate = certificate;
        //    NotifyOfPropertyChange(() => Certificate);
        //}

        private void SaveCertificate()
        {
        }
    }
}