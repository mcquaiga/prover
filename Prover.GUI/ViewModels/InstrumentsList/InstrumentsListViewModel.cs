using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            GetInstruments(Guid.Empty);
           
        }
        public ObservableCollection<InstrumentViewModel> Instruments { get; set; }

        private void GetInstruments(Guid certificateGuid)
        {
            if (certificateGuid == Guid.Empty)
            {
                using (var store = _container.Resolve<IInstrumentStore<Instrument>>())
                {
                    Instruments = new ObservableCollection<InstrumentViewModel>();
                    var instruments = store.Query()
                                        .Where(x => x.CertificateGuid == Guid.Empty)
                                        .ToList();
                    instruments.ForEach(i => Instruments.Add(new InstrumentViewModel(i)));
                }
            }

            NotifyOfPropertyChange(() => Instruments);
        }
    }
}
