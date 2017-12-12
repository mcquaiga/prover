using Akavache;
using Autofac;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.TestActions.PreTestActions;
using Prover.Core.VerificationTests.VolumeVerification;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public CoreBootstrapper()
        {
            BlobCache.ApplicationName = "EvcProver";

            //Database registrations
            Builder.RegisterInstance(new ProverContext());
            Builder.RegisterType<InstrumentStore>().As<IInstrumentStore<Instrument>>();
            Builder.RegisterType<CertificateStore>().As<ICertificateStore<Certificate>>();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            
            ////QA Test Runs
            Builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1)).Named<IDInOutBoard>("TachDaqBoard");
            Builder.Register(c => new TachometerService(SettingsManager.SettingsInstance.TachCommPort, c.ResolveNamed<IDInOutBoard>("TachDaqBoard")))
                .As<TachometerService>();

            Builder.RegisterType<AutoVolumeTestManager>().As<VolumeTestManager>();
            Builder.RegisterType<AverageReadingStabilizer>().As<IReadingStabilizer>();
            Builder.RegisterType<QaRunTestManager>().As<IQaRunTestManager>();

            Builder.Register(c =>
            {
                var resetItems = SettingsManager.SettingsInstance.TocResetItems;
                return new TocItemUpdater(resetItems);
            }).As<IPreTestAction>();

            Task.Run(SettingsManager.RefreshSettings);
        }

        public ContainerBuilder Builder { get; } = new ContainerBuilder();
    }
}