using System;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro;
using System.Data.Entity;
using Autofac;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Migrations;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using IContainer = Autofac.IContainer;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public IContainer Container { get; }

        public CoreBootstrapper()
        {
            var builder = new ContainerBuilder();
            
            //Database registrations
            builder.RegisterInstance(new ProverContext());
            builder.RegisterType<InstrumentStore>().As<IInstrumentStore<Instrument>>();
            builder.RegisterType<CertificateStore>().As<ICertificateStore<Certificate>>();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            
            //
            builder.RegisterType<EventAggregator>().As<EventAggregator>();

            //EVC Communcation
            //Container.RegisterType<CommPort, SerialPort>("SerialPort", new InjectionFactory(c => new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate)));
            builder.RegisterType<SerialPort>().As<CommPort>();
            Container = builder.Build();

            var commPortFactory = Container.Resolve<SerialPort.Factory>();
            var commPort = commPortFactory.Invoke(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);


            //Container.RegisterType<EvcCommunicationClient, HoneywellClient>(
            //    new InjectionFactory(c => new HoneywellClient(c.Resolve<CommPort>("SerialPort"))));

            ////QA Test Runs
            //Container.RegisterType<IQaRunTestManager, QaRunTestManager>();
            //Container.RegisterType<IReadingStabilizer, ReadingStabilizer>();

            SettingsManager.RefreshSettings();
        }
    }
}
