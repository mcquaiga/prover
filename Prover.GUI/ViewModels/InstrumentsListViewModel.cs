using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro.ReactiveUI;

namespace Prover.GUI.ViewModels
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
            else
            {
                //Instruments =
                //    _container.Resolve<IInstrumentStore<Instrument>>()
                //        .Query()
                //        .Where(x => x.CertificateGuid == certificateGuid)
                //        .ToList();
            }

            NotifyOfPropertyChange(() => Instruments);
        }
    }
}
