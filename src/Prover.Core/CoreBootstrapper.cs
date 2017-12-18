using System.Threading.Tasks;
using Autofac;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.Core.Shared.Data;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.VolumeVerification;
using LogManager = NLog.LogManager;

namespace Prover.Core
{
    public class CoreBootstrapper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void RegisterServices(ContainerBuilder builder)
        {
            SettingsManager.RefreshSettings().Wait();

            SetupDatabase(builder);

            RegisterCommunications(builder);

            builder.RegisterBuildCallback(container =>
            {
                SettingsManager.Initialize(container.Resolve<KeyValueStore>());
                SettingsManager.RefreshSettings().Wait();
            });
        }

        private static void RegisterCommunications(ContainerBuilder builder)
        {
            //EVC Communcation
            Task.Run(ItemHelpers.LoadInstrumentTypes);

            builder.Register(c => new SerialPort(SettingsManager.LocalSettingsInstance.InstrumentCommPort, SettingsManager.LocalSettingsInstance.InstrumentBaudRate))
                .Named<ICommPort>("SerialPort");

            builder.Register(c => new IrDAPort())
                .Named<ICommPort>("IrDAPort");

            //builder.Register(c =>
            //{
            //    var instrument = HoneywellInstrumentTypes.GetByName(SettingsManager.LocalSettingsInstance.LastInstrumentTypeUsed);

            //    return SettingsManager.LocalSettingsInstance.InstrumentUseIrDaPort
            //        ? new HoneywellClient(c.ResolveNamed<ICommPort>("IrDAPort"), instrument)
            //        : new HoneywellClient(c.ResolveNamed<ICommPort>("SerialPort"), instrument);
            //}).As<EvcCommunicationClient>();

            //QA Test Runs
            builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1)).Named<IDInOutBoard>("TachDaqBoard");
            builder.Register(c => new TachometerService(SettingsManager.LocalSettingsInstance.TachCommPort, c.ResolveNamed<IDInOutBoard>("TachDaqBoard")))
                .As<TachometerService>();

            builder.RegisterType<AutoVolumeTestManager>();
            builder.RegisterType<ManualVolumeTestManager>();

            builder.RegisterType<AverageReadingStabilizer>().As<IReadingStabilizer>();
            builder.RegisterType<QaRunTestManager>().As<IQaRunTestManager>();
        }

        private static void SetupDatabase(ContainerBuilder builder)
        {
            //Database registrations
            Log.Debug("Started initializing database...");
            builder.RegisterType<ProverContext>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();                    
            
            builder.Register(c => new KeyValueStore(c.Resolve<ProverContext>()))
                .As<KeyValueStore>()
                .InstancePerDependency();

            builder.RegisterType<InstrumentStore>().As<IProverStore<Instrument>>()
                .InstancePerDependency();
            builder.RegisterType<TestRunService>()
                .SingleInstance();

            builder.Register(c => new ProverStore<Client>(c.Resolve<ProverContext>()))
                .As<IProverStore<Client>>()
                .InstancePerDependency();
            builder.Register(c => new ProverStore<ClientCsvTemplate>(c.Resolve<ProverContext>())).As<IProverStore<ClientCsvTemplate>>()
                .InstancePerDependency();
            builder.RegisterType<ClientService>();

            builder.RegisterType<CertificateStore>().As<IProverStore<Certificate>>()
                .InstancePerDependency();
            builder.RegisterType<CertificateService>().As<ICertificateService>();

            Log.Debug("Completed initializing database...");
        }
    }
}