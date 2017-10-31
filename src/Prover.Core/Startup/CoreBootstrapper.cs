using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Communication;
using Prover.Core.Exports;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.VolumeVerification;
using LogManager = NLog.LogManager;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger(); 

        public CoreBootstrapper()
        {
            SetupDatabase();

            //EVC Communcation
            Task.Run(ItemHelpers.LoadInstruments);

            Builder.Register(c => new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate))
                .Named<ICommPort>("SerialPort");

            Builder.Register(c => new IrDAPort())
                .Named<ICommPort>("IrDAPort");

            Builder.Register(c =>
                {
                    var instrument = HoneywellInstrumentTypes.GetByName(SettingsManager.SettingsInstance.LastInstrumentTypeUsed);

                    return SettingsManager.SettingsInstance.InstrumentUseIrDAPort
                        ? new HoneywellClient(c.ResolveNamed<ICommPort>("IrDAPort"), instrument)
                        : new HoneywellClient(c.ResolveNamed<ICommPort>("SerialPort"), instrument);
                })
                .As<EvcCommunicationClient>();

            //QA Test Runs
            Builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1)).Named<IDInOutBoard>("TachDaqBoard");
            Builder.Register(c => new TachometerService(SettingsManager.SettingsInstance.TachCommPort, c.ResolveNamed<IDInOutBoard>("TachDaqBoard")))
                .As<TachometerService>();

            Builder.RegisterType<AutoVolumeTestManager>();          

            Builder.RegisterType<AverageReadingStabilizer>().As<IReadingStabilizer>();
            Builder.RegisterType<QaRunTestManager>().As<IQaRunTestManager>();

            SettingsManager.RefreshSettings().Wait();
        }

        private void SetupDatabase()
        {
            //Database registrations
            _log.Debug("Started initializing database...");
            _log.Debug("    Running Migrations.");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());

            Builder.RegisterType<ProverContext>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();

            Builder.RegisterType<InstrumentStore>()
                .As<IProverStore<Instrument>>()
                .SingleInstance();
            Builder.RegisterType<TestRunService>();

            Builder.RegisterType<ClientStore>()
                .As<IProverStore<Client>>()
                .SingleInstance();
            Builder.Register(c => new ProverStore<ClientCsvTemplate>(c.Resolve<ProverContext>()))
                .As<IProverStore<ClientCsvTemplate>>()
                .SingleInstance();
            Builder.RegisterType<ClientService>();

            Builder.RegisterType<CertificateStore>()
                .As<IProverStore<Certificate>>()
                .SingleInstance();
            Builder.RegisterType<CertificateService>()
                .As<ICertificateService>();

            _log.Debug("Completed initializing database...");
        }

        public ContainerBuilder Builder { get; } = new ContainerBuilder();
    }
}