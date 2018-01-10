using System.Threading.Tasks;
using Autofac;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.ExternalDevices;
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
            SetupDatabase(builder);

            RegisterCommunications(builder);

            builder.RegisterBuildCallback(async container =>
            {
                await ItemHelpers.LoadInstrumentTypes();
            });
        }

        private static void RegisterCommunications(ContainerBuilder builder)
        {
            //EVC Communcation
            builder.Register(c =>
                {
                    var ss = c.Resolve<ISettingsService>();
                    return new SerialPort(ss.Local.InstrumentCommPort, ss.Local.InstrumentBaudRate);
                })
                .Named<ICommPort>("SerialPort");

            builder.Register(c => new IrDAPort())
                .Named<ICommPort>("IrDAPort");

            //QA Test Runs
            builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1))
                .Named<IDInOutBoard>("TachDaqBoard");

            builder.Register(c =>
                {
                    var tach = c.Resolve<ISettingsService>().Local.TachIsNotUsed == false
                        ? c.Resolve<ISettingsService>().Local.TachCommPort
                        : string.Empty;
                    return new TachometerService(tach, c.ResolveNamed<IDInOutBoard>("TachDaqBoard"));
                })
                .As<TachometerService>();

            builder.RegisterType<AutoVolumeTestManager>();
            builder.RegisterType<ManualVolumeTestManager>();

            builder.RegisterType<AverageReadingStabilizer>()
                .As<IReadingStabilizer>();

            builder.RegisterType<QaRunTestManager>()
                .As<IQaRunTestManager>();
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
          
            builder.RegisterType<SettingsService>()
                .AsImplementedInterfaces()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterType<InstrumentStore>().As<IProverStore<Instrument>>()
                .InstancePerDependency();
            builder.RegisterType<TestRunService>()
                .SingleInstance();

            builder.Register(c => new ProverStore<Client>(c.Resolve<ProverContext>()))
                .As<IProverStore<Client>>()
                .InstancePerDependency();
            builder.Register(c => new ProverStore<ClientCsvTemplate>(c.Resolve<ProverContext>())).As<IProverStore<ClientCsvTemplate>>()
                .InstancePerDependency();
            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<CertificateStore>().As<IProverStore<Certificate>>()
                .InstancePerDependency();
            builder.RegisterType<CertificateService>().As<ICertificateService>();

            Log.Debug("Completed initializing database...");
        }
    }
}