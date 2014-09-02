using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public IList<Instrument> Instruments { get; set; }

        private void GetInstruments(Guid certificateGuid)
        {
            if (certificateGuid == Guid.Empty)
            {
                Instruments =
                    _container.Resolve<IInstrumentStore<Instrument>>()
                        .Query()
                        .Where(x => x.CertificateGuid == Guid.Empty)
                        .ToList();
            }
            else
            {
                Instruments =
                    _container.Resolve<IInstrumentStore<Instrument>>()
                        .Query()
                        .Where(x => x.CertificateGuid == certificateGuid)
                        .ToList();
            }

            NotifyOfPropertyChange(() => Instruments);
        }
    }
}
