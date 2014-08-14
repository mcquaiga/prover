using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public IUnityContainer Container;

        public CoreBootstrapper()
        {
            Container = new UnityContainer();
            Container.RegisterInstance<IInstrumentStore<Instrument>>(new InstrumentStore());           
        }
    }
}
