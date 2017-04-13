using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.ViewModels.InstrumentsList;

namespace Prover.Modules.CertificatesUi.Screens.Certificates
{
    public class InstrumentsListViewModel : ReactiveScreen
    {
        private readonly IProverStore<Instrument> _instrumentStore;

        public InstrumentsListViewModel(IProverStore<Instrument> instrumentStore)
        {
            _instrumentStore = instrumentStore;

            GetInstrumentsByCertificateId(null);
        }
        public ObservableCollection<InstrumentViewModel> InstrumentItems { get; set; }

        public void GetInstrumentsByCertificateId(Guid? certificateGuid)
        {

                int count = 1;
                InstrumentItems = new ObservableCollection<InstrumentViewModel>();
                var instruments = _instrumentStore.Query()
                                    .Where(x => x.CertificateId == certificateGuid)
                                    .OrderBy(i => i.TestDateTime)
                                    .ToList();
                instruments.ForEach(i =>
                {
                    InstrumentItems.Add(new InstrumentViewModel(i, count));
                    count++;
                });

            NotifyOfPropertyChange(() => InstrumentItems);
        }

        public void GetInstrumentsWithNoCertificateLastWeek()
        {

                InstrumentItems = new ObservableCollection<InstrumentViewModel>();
                var dateFilter = DateTime.Now.AddDays(-7);
                var instruments = _instrumentStore.Query()
                                    .Where(x => x.CertificateId == null && x.TestDateTime >= dateFilter)
                                    .OrderBy(i => i.TestDateTime)
                                    .ToList();
                instruments.ForEach(i => InstrumentItems.Add(new InstrumentViewModel(i)));
            
            NotifyOfPropertyChange(() => InstrumentItems);
        }

        public void GetInstrumentsWithNoCertificateLastMonth()
        {

                InstrumentItems = new ObservableCollection<InstrumentViewModel>();
                var dateFilter = DateTime.Now.AddDays(-30);
                var instruments = _instrumentStore.Query()
                                    .Where(x => x.CertificateId == null && x.TestDateTime >= dateFilter)
                                    .OrderBy(i => i.TestDateTime)
                                    .ToList();
                instruments.ForEach(i => InstrumentItems.Add(new InstrumentViewModel(i)));
            
            NotifyOfPropertyChange(() => InstrumentItems);
        }
    }
}
