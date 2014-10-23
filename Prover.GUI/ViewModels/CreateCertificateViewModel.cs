using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.ViewModels.InstrumentsList;

namespace Prover.GUI.ViewModels
{
    public class CreateCertificateViewModel: ReactiveScreen
    {
        private IUnityContainer _container;

        public CreateCertificateViewModel(IUnityContainer container)
        {
            _container = container;
            InstrumentsListViewModel = new InstrumentsListViewModel(_container);
        }

        public InstrumentsListViewModel InstrumentsListViewModel { get; private set; }

        public Certificate Certificate { get; set; }

        public string CreatedDateTime { get; set; }
        public string VerificationType { get; set; }
        public string TestedBy { get; set; }
        
        public void CreateCertificate()
        {
            Certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                TestedBy = TestedBy,
                Instruments = new Collection<Instrument>()
            };

            var selectedInstruments = InstrumentsListViewModel.InstrumentItems.Where(x => x.IsSelected == true).ToList();
            selectedInstruments.ForEach(i =>
            {
                i.Instrument.CertificateId = Certificate.Id;
                i.Instrument.Certificate = Certificate;
                Certificate.Instruments.Add(i.Instrument);

            });

            var certificateStore = new CertificateStore(_container);
            certificateStore.Upsert(Certificate);
        }
    }
}
