using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Storage.Interfaces;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Tests.Application.Services;

namespace Prover.Application.Services.Tests
{
    [TestClass()]
    public class VerificationDashboardServiceTests
    {
        private CompositeDisposable _cleanup;
        private static DeviceInstance _device;
        private static DeviceType _deviceType;
        private static IDeviceRepository _repo;

        private static IAsyncRepository<EvcVerificationTest> _testRepo;
        private static IEntityDataCache<EvcVerificationTest> _cache;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _repo = StorageTestsInitialize.DeviceRepo;
            _deviceType = _repo.GetByName("Mini-Max");

            _testRepo = StorageTestsInitialize.TestRepo;
            _cache = StorageTestsInitialize.VerificationCache;
            await Task.CompletedTask;
        }

        [TestMethod()]
        public void VerificationDashboardServiceTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            //Assert.Fail();
        }
    }
}