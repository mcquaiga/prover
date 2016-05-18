using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro;
using System.Data.Entity;
using Prover.Core.Migrations;
using Prover.Core.Settings;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public IUnityContainer Container;

        public CoreBootstrapper()
        {
            Container = new UnityContainer();
 
            Container.RegisterInstance(new ProverContext());
            Container.RegisterInstance<IInstrumentStore<Instrument>>(new InstrumentStore(Container));
            Container.RegisterInstance<ICertificateStore<Certificate>>(new CertificateStore(Container));
            
            SettingsManager.RefreshSettings();

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
        }
    }
}
