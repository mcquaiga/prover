using System.Data.Entity;
using Autofac;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Storage;
using Prover.Core.VerificationTests;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public CoreBootstrapper()
        {
            //Database registrations
            Builder.RegisterInstance(new ProverContext());
            Builder.RegisterType<InstrumentStore>().As<IInstrumentStore<Instrument>>();
            Builder.RegisterType<CertificateStore>().As<ICertificateStore<Certificate>>();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());

            Builder.RegisterType<EventAggregator>().As<EventAggregator>();

            //EVC Communcation
            Builder.Register(
                c =>
                    new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort,
                        SettingsManager.SettingsInstance.InstrumentBaudRate)).Named<CommPort>("SerialPort");

            Builder.Register(c => new HoneywellClient(c.ResolveNamed<CommPort>("SerialPort")))
                .As<EvcCommunicationClient>();

            ////QA Test Runs
            Builder.RegisterType<AverageReadingStabilizer>().As<IReadingStabilizer>();
            Builder.RegisterType<QaRunTestManager>().As<IQaRunTestManager>();

            SettingsManager.RefreshSettings();
        }

        public ContainerBuilder Builder { get; } = new ContainerBuilder();
    }
}