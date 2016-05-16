using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.GUI.ViewModels.InstrumentsList
{
    public class InstrumentsListViewModel : ReactiveScreen
    {
        private IUnityContainer _container;
        public InstrumentsListViewModel(IUnityContainer container)
        {
            _container = container;
            GetInstrumentsByCertificateId(null);
           
        }
        public ObservableCollection<InstrumentTestGridViewModel> InstrumentItems { get; set; } = new ObservableCollection<InstrumentTestGridViewModel>();

        public void GetInstrumentsByCertificateId(Guid? certificateGuid)
        {
            GetInstrumentVerificationTests(x => x.CertificateId == certificateGuid);
        }

        public void GetInstrumentsWithNoCertificateLastMonth()
        {
            var dateFilter = DateTime.Now.AddDays(-30);
            GetInstrumentVerificationTests(x => x.CertificateId == null && x.TestDateTime >= dateFilter);
        }

        public void GetInstrumentsWithNoCertificateLastWeek()
        {
            var dateFilter = DateTime.Now.AddDays(-7);
            GetInstrumentVerificationTests(x => x.CertificateId == null && x.TestDateTime >= dateFilter);
        }

        private void GetInstrumentVerificationTests(Func<Instrument, bool> whereFunc)
        {
            InstrumentItems.Clear();
            
            using (var store = _container.Resolve<IInstrumentStore<Instrument>>())
            {
                var instruments = store.Query()
                                    .Where(whereFunc)
                                    .OrderBy(i => i.TestDateTime)
                                    .ToList();

                instruments.ForEach(i => InstrumentItems.Add(new InstrumentTestGridViewModel(_container, i)));
            }

            NotifyOfPropertyChange(() => InstrumentItems);
        }
    }
}
