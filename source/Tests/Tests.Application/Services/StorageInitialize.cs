using System;
using System.Threading.Tasks;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Infrastructure;
using Prover.Infrastructure.KeyValueStore;

namespace Tests.Application.Services
{
    [TestClass]
    public class StorageInitialize
    {
        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            Repo = new DeviceRepository();
            await Repo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);
            await Repo.UpdateCachedTypes(RometJsonDeviceTypeDataSource.Instance);

            TestRepo = new VerificationsLiteDbRepository(StorageDefaults.Database, Repo);
            ModelService = new EvcVerificationTestService(TestRepo);
            ViewModelService = new VerificationTestService(TestRepo, new VerificationViewModelFactory(), null, null);
            Console.WriteLine("AssemblyInitialize");
        }

        public static VerificationTestService ViewModelService { get; private set; }

        public static EvcVerificationTestService ModelService { get; private set; }

        public static VerificationsLiteDbRepository TestRepo { get; private set; }

        public static DeviceRepository Repo { get; private set; }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }
    }
}