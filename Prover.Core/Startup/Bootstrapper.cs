using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Startup
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            var container = new UnityContainer();

            container.RegisterInstance<IInstrumentStore<Instrument>>(new InstrumentStore());           
        }
    }
}
