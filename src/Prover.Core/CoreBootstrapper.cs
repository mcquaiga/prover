using Autofac;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.ExternalDevices;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.Core.Shared.Components;
using Prover.Core.Shared.Data;
using Prover.Core.Shared.Domain;
using Prover.Core.Storage;
using Prover.Core.Testing;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.TestActions.PreTestActions;
using Prover.Core.VerificationTests.VolumeVerification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogManager = NLog.LogManager;

namespace Prover.Core
{
    public static class CoreBootstrapper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void RegisterServices(ContainerBuilder builder)
        {
   
            SetupDatabase(builder);                       

            builder.RegisterType<SettingsService>()                              
                .As<ISettingsService>()     
                .As<IStartable>()
                .SingleInstance();          

            RegisterCommunications(builder);

            //builder.RegisterBuildCallback(_ => AsyncUtil.RunSync(ItemHelpers.LoadInstrumentTypes));
            builder.RegisterBuildCallback(_ => Task.Run(ItemHelpers.GetInstrumentDefinitions));
        }

        private static void RegisterCommunications(ContainerBuilder builder)
        {    
            builder.RegisterType<SerialPort>();

            builder.RegisterType<IrDAPort>()
                .Named<ICommPort>("IrDAPort");

            builder.Register<Func<string, int, ICommPort>>(c =>
            {     
                var resolve = c;
                return (port, baud) =>
                {
                    if (string.IsNullOrEmpty(port))
                    {
                        return new IrDAPort();
                    }

                    return new SerialPort(port, baud);                    
                };              
            });           

            //QA Test Runs
            builder.Register(c => DInOutBoardFactory.CreateBoard(0, 0, 1))
                .As<IDInOutBoard>();

            builder.Register(c =>
                {
                    var tach = c.Resolve<ISettingsService>().Local.TachIsNotUsed == false
                        ? c.Resolve<ISettingsService>().Local.TachCommPort
                        : string.Empty;
                    return new TachometerService(tach, c.Resolve<IDInOutBoard>());
                })
                .As<TachometerService>();           

            builder.RegisterType<MechanicalAutoVolumeTestManager>();
            builder.RegisterType<RotaryAutoVolumeTestManager>();
            builder.RegisterType<ManualVolumeTestManager>();

            builder.RegisterType<AverageReadingStabilizer>()
                .As<IReadingStabilizer>();

            builder.RegisterType<QaRunTestManager>()
                .As<IQaRunTestManager>();

            builder.RegisterType<RotaryStressTest>();

            RegisterTestActions(builder);
        }

        private static void RegisterTestActions(ContainerBuilder builder)
        {
            builder.RegisterType<TestActionsManager>().As<ITestActionsManager>();

            builder.Register(c =>
            {
                var resetItems = c.Resolve<ISettingsService>().Shared.TestSettings.TocResetItems;
                return new TocItemUpdaterAction(resetItems);
            })
            .As<IPreVolumeTestAction>()
            .Named<IPreVolumeTestAction>("TocVolPulsesWaitingReset");

            builder.Register(c =>
            {
                var resetItems = new Dictionary<int, string>
                {
                    {5, "0" },
                    {6, "0" },
                    {7, "0" }
                };
                return new ItemUpdaterAction(resetItems);
            })
            .As<IPreVolumeTestAction>()
            .Named<IPreVolumeTestAction>("PulseOutputWaitingReset");


        }

        private static void SetupDatabase(ContainerBuilder builder)
        {
            //Database registrations           
            builder.RegisterType<ProverContext>()
                .AsSelf()
                .As<IStartable>()
                .SingleInstance();

            //Register all types of EntityWithId as ProverStore<T>
            var asm = Assembly.GetExecutingAssembly();
            var stores = asm.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(EntityWithId)) && !t.IsAbstract)
                .Select(t => typeof(ProverStore<>).MakeGenericType(new[] { t }))
                .ToList();

            builder.RegisterTypes(stores.ToArray())
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterType<KeyValueStore>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<InstrumentStore>()
                .AsSelf()
                .As<IProverStore<Instrument>>()
                .InstancePerDependency();

            builder.RegisterType<TestRunService>()
               .SingleInstance();

            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<CertificateService>().As<ICertificateService>();
        }
    }
}