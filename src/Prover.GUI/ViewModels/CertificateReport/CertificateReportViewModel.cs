using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.ViewModels.InstrumentsList;
using Prover.GUI.Views.CertificateReport;

namespace Prover.GUI.ViewModels.CertificateReport
{
    public class CertificateReportViewModel : ReactiveScreen
    {
        private readonly IUnityContainer _container;

        public CertificateReportViewModel(IUnityContainer container, Certificate certificate)
        {
            _container = container;
            
            LoadCertificate(certificate);
        }

        public Certificate Certificate { get; set; }

        public string CertificateDate { get { return Certificate.CreatedDateTime.ToShortDateString(); } }

        public InstrumentsListViewModel InstrumentsView { get; set; }

        private void LoadCertificate(Certificate certificate)
        {
            InstrumentsView = new InstrumentsListViewModel(_container);
            InstrumentsView.GetInstrumentsByCertificateId(certificate.Id);
            Certificate = certificate;
            NotifyOfPropertyChange(() => Certificate);
        }

        private void SaveCertificate()
        {
            
        }
    }
}
