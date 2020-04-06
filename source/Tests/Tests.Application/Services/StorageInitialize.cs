using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;

namespace Tests.Application.Services
{
    [TestClass]
    public class StorageTestsInitialize
    {
        public static VerificationTestService ViewModelService { get; private set; }

        public static VerificationsLiteDbRepository TestRepo { get; private set; }

        public static DeviceRepository Repo { get; private set; }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            Repo = new DeviceRepository();
            await Repo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);
            await Repo.UpdateCachedTypes(RometJsonDeviceTypeDataSource.Instance);

            TestRepo = new VerificationsLiteDbRepository(StorageDefaults.Database, Repo);

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
            new VerificationTestService(TestRepo, new VerificationViewModelFactory(), null, null);

        public static void DropCollection()
        {
            StorageDefaults.Database.DropCollection("EvcVerificationTest");
        }
    }
}