using System;
using System.ComponentModel;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Caliburn.Micro;
using System.Data.Entity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
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
            
            //Event Aggregator
            Container.RegisterType<IEventAggregator, EventAggregator>();

            //EVC Communcation
            Container.RegisterType<CommPort, SerialPort>("SerialPort",
                new InjectionFactory(c => new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate)));

            Container.RegisterType<EvcCommunicationClient, HoneywellClient>(
                new InjectionFactory(c => new HoneywellClient(c.Resolve<CommPort>("SerialPort"))));

            Container.RegisterType<TachometerService>(
                new InjectionFactory(t => new TachometerService(SettingsManager.SettingsInstance.TachCommPort, DInOutBoardFactory.CreateBoard(0, 0, 1))));

            //QA Test Runs
            Container.RegisterType<IQaRunTestManager, QaRunTestManager>();
            Container.RegisterType<IReadingStabilizer, AverageReadingStabilizer>();

            SettingsManager.RefreshSettings();
        }
    }
}
