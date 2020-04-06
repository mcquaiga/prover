using System.Threading.Tasks;
using DeepEqual.Syntax;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels.Volume.Factories;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
using Tests.Shared;

namespace Prover.Application.Services.Tests
{
    [TestClass()]
    public class VerificationViewModelServiceTests
    {
        
    }
}

namespace Tests.Application.Services
{
    [TestClass]
    public class VerificationViewModelServiceTests
    {
        private static TestSchedulers _schedulers = new TestSchedulers();
        private static DeviceInstance _device;
        private static DeviceType _deviceType;
        private static Mock<DeviceInstance> _instance;
        private static IDeviceRepository _repo;

        private static IVerificationViewModelFactory _verificationViewModelFactory = new VerificationViewModelFactory();
        private static readonly Mock<IAsyncRepository<EvcVerificationTest>> _repoMock =
            new Mock<IAsyncRepository<EvcVerificationTest>>();

        private static IVerificationTestService _service;

        [TestMethod]
        public async Task CreateVerificationTestFromViewModelTest()
        {
            var newTest = _service.NewVerification(_device);

            var model = _service.CreateModel(newTest);
            Assert.AreEqual(newTest.Id, model.Id);

            var viewModel = await _service.GetViewModel(model);
            //newTest.WithDeepEqual(viewModel)
            //    .IgnoreSourceProperty(t => t.Device.DeviceType)
            //    .Assert();

            var model2 = _service.CreateModel(viewModel);
            model2.ShouldDeepEqual(model);

            Assert.IsTrue(model2.IsDeepEqual(model));
            //Assert.IsTrue(newTest.IsDeepEqual(viewModel));
        }

        [TestMethod]
        public void GetVerificationTestsTest()
        {
        }

        [TestInitialize]
        public async Task TestInit()
        {
            var items = ItemFiles.MiniMaxItemFile;
            _device = _deviceType.CreateInstance(items);

            await Task.CompletedTask;
        }

        [TestMethod]
        public void NewTestTest()
        {
            var newTest = _service.NewVerification(_device);

            Assert.IsNotNull(newTest);
        }

      

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _repo = StorageTestsInitialize.Repo;
            _deviceType = _repo.GetByName("Mini-Max");

            _service = StorageTestsInitialize.ViewModelService;
            _instance = new Mock<DeviceInstance>();


        }
    }
}