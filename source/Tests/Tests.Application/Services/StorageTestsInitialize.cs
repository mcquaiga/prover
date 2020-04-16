using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Factories;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;

namespace Tests.Application.Services
{
    [TestClass]
    public class StorageTestsInitialize
    {
        public static VerificationTestService ViewModelService { get; private set; }

        public static VerificationsLiteDbRepository TestRepo { get; private set; }

        public static IDeviceRepository DeviceRepo { get; private set; }

        public static ILogger Logger { get; } = new DebugLoggerProvider().CreateLogger("StorageTests");
        public static ILoggerProvider LoggerProvider { get; } = new DebugLoggerProvider();
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory(new []{new DebugLoggerProvider()});

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            //DeviceRepo = new DeviceRepository();
            //await DeviceRepo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);
            //await DeviceRepo.UpdateCachedTypes(RometJsonDeviceTypeDataSource.Instance);
            DeviceRepo = DeviceRepository.Instance;

            StorageDefaults.CreateLiteDb();
            TestRepo = new VerificationsLiteDbRepository(StorageDefaults.Database, DeviceRepo);
            ViewModelService = CreateVerificationTestService();
            Console.WriteLine("AssemblyInitialize");
        }

        public static async Task<EvcVerificationViewModel> CreateAndSaveNewTest(DeviceInstance device,
            IVerificationTestService testService = null)
        {
            testService ??= ViewModelService;
            var newTest = testService.NewVerification(device);

            await testService.AddOrUpdate(newTest);
            return newTest;
        }

        public static async Task<EvcVerificationViewModel> CreateAndSaveNewTest(DeviceType deviceType,
            Dictionary<int, string> itemValues, IVerificationTestService testService = null) =>
            await CreateAndSaveNewTest(deviceType.CreateInstance(itemValues), testService);

        public static async Task<ICollection<EvcVerificationViewModel>> CreateAndSaveNewTests(DeviceType deviceType,
            Dictionary<int, string> itemValues, int numberOfTest, IVerificationTestService testService = null)
        {
            var records = new List<EvcVerificationViewModel>();
            var tasks = new List<Task<EvcVerificationViewModel>>();
            for (var i = 0; i < numberOfTest; i++)
            {
                var device = deviceType.CreateInstance(itemValues);
                records.Add(await CreateAndSaveNewTest(device, testService));
            }

            return records;
        }

        public static VerificationTestService CreateVerificationTestService() =>
            new VerificationTestService(LoggerFactory.CreateLogger<VerificationTestService>(), TestRepo, new VerificationViewModelFactory(null), null);

        public static void DropCollection()
        {
            StorageDefaults.Database.DropCollection("EvcVerificationTest");
        }
    }
}