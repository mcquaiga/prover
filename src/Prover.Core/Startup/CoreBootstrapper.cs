using System.Data.Entity;
using Autofac;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Communication;
using Prover.Core.Exports;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Migrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.VolumeVerification;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        private Logger _log = LogManager.GetCurrentClassLogger(); 

        public CoreBootstrapper()
        {
            //Database registrations
            _log.Debug("Started initializing database...");            
            _log.Debug("    Running Migrations.");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            
            Builder.Register(c => new ProverContext())
                .SingleInstance();
                
            Builder.Register(c => new InstrumentStore(c.Resolve<ProverContext>()))
                .As<IProverStore<Instrument>>()
                .SingleInstance();

            _log.Debug("Completed initializing database...");

            //EVC Communcation
            Builder.Register(c => new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate))
                .Named<CommPort>("SerialPort");

            Builder.Register(c => new HoneywellClient(c.ResolveNamed<CommPort>("SerialPort")))
                .As<EvcCommunicationClient>();

            //QA Test Runs
            Builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1)).Named<IDInOutBoard>("TachDaqBoard");
            Builder.Register(c => new TachometerService(SettingsManager.SettingsInstance.TachCommPort, c.ResolveNamed<IDInOutBoard>("TachDaqBoard")))
                .As<TachometerService>();

            Builder.RegisterType<AutoVolumeTestManagerBase>().As<VolumeTestManagerBase>();
            Builder.RegisterType<AverageReadingStabilizer>().As<IReadingStabilizer>();
            Builder.RegisterType<QaRunTestManager>().As<IQaRunTestManager>();

            SettingsManager.RefreshSettings().Wait();
        }

        public ContainerBuilder Builder { get; } = new ContainerBuilder();
    }
}