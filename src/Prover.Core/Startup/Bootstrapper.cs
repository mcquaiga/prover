using System;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro;
using System.Data.Entity;
using System.Reflection;
using Autofac;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Migrations;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using ReactiveUI;
using ReactiveUI.Autofac;
using Splat;
using IContainer = Autofac.IContainer;

namespace Prover.Core.Startup
{
    public class CoreBootstrapper
    {
        public ContainerBuilder Builder { get; } = new ContainerBuilder();

        public CoreBootstrapper()
        {
            //Database registrations
            Builder.RegisterInstance(new ProverContext());
            Builder.RegisterType<InstrumentStore>().As<IInstrumentStore<Instrument>>();
            Builder.RegisterType<CertificateStore>().As<ICertificateStore<Certificate>>();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());

            Builder.RegisterType<EventAggregator>().As<EventAggregator>();

            //EVC Communcation
            //Container.RegisterType<CommPort, SerialPort>("SerialPort", new InjectionFactory(c => new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate)));
            Builder.RegisterType<SerialPort>().As<CommPort>();
            
            //var commPortFactory = Container.Resolve<SerialPort.Factory>();
            //var commPort = commPortFactory.Invoke(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
            
            //Container.RegisterType<EvcCommunicationClient, HoneywellClient>(
            //    new InjectionFactory(c => new HoneywellClient(c.Resolve<CommPort>("SerialPort"))));

            ////QA Test Runs
            //Container.RegisterType<IQaRunTestManager, QaRunTestManager>();
            //Container.RegisterType<IReadingStabilizer, ReadingStabilizer>();

            SettingsManager.RefreshSettings();
        }
    }
}
