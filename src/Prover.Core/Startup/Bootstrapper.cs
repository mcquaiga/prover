using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro;
using System.Data.Entity;
using Prover.CommProtocol.Common.IO;
using Prover.Core.Migrations;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public IUnityContainer Container;

        public CoreBootstrapper()
        {
            Container = new UnityContainer();
 
            //Database registrations
            Container.RegisterInstance(new ProverContext());
            Container.RegisterType<IInstrumentStore<Instrument>, InstrumentStore>();
            Container.RegisterType<ICertificateStore<Certificate>, CertificateStore>();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());

            //
            Container.RegisterType<IEventAggregator, EventAggregator>();
            Container.RegisterType<IReadingStabilizer, ReadingStabilizer>();

            SettingsManager.RefreshSettings();
        }
    }
}
