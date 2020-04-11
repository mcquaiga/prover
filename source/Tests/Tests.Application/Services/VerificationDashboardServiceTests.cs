using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Services;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
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
            _cache  =StorageTestsInitialize.ViewModelService;
            await Task.CompletedTask;
        }

        [TestMethod()]
        public void VerificationDashboardServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }
    }
}