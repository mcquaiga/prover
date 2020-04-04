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

namespace Tests.Application.Services
{
    [TestClass]
    public class VerificationViewModelServiceTests
    {
        private DeviceInstance _device;
        private DeviceType _deviceType;
        private Mock<DeviceInstance> _instance;
        private IDeviceRepository _repo;

        private IVerificationViewModelFactory _verificationViewModelFactory = new VerificationViewModelFactory();
        private readonly Mock<IAsyncRepository<EvcVerificationTest>> _repoMock =
            new Mock<IAsyncRepository<EvcVerificationTest>>();

        private IVerificationTestService _service;

        [TestMethod]
        public async Task CreateVerificationTestFromViewModelTest()
        {
            var newTest = _service.NewTest(_device);

            var model = _service.CreateVerificationTestFromViewModel(newTest);
            Assert.AreEqual(newTest.Id, model.Id);

            var viewModel = await _service.GetVerificationTest(model);
            //newTest.WithDeepEqual(viewModel)
            //    .IgnoreSourceProperty(t => t.Device.DeviceType)
            //    .Assert();

            var model2 = _service.CreateVerificationTestFromViewModel(viewModel);
            model2.ShouldDeepEqual(model);

            //Assert.IsTrue(model2.IsDeepEqual(model));
            //Assert.IsTrue(newTest.IsDeepEqual(viewModel));
        }

        [TestMethod]
        public void GetVerificationTestsTest()
        {
        }

        [TestInitialize]
        public async Task Init()
        {
            _service = new VerificationTestService(_repoMock.Object, _verificationViewModelFactory, null, null);
            _instance = new Mock<DeviceInstance>();

            _repo = new DeviceRepository();
            await _repo.UpdateCachedTypes(MiJsonDeviceTypeDataSource.Instance);

            _deviceType = _repo.GetByName("Mini-Max");
            var items = ItemFiles.MiniMaxItemFile;
            _device = _deviceType.CreateInstance(items);
        }

        [TestMethod]
        public void NewTestTest()
        {
            var newTest = _service.NewTest(_device);

            Assert.IsNotNull(newTest);
        }

        [TestMethod]
        public void VerificationViewModelServiceTest()
        {
        }
    }
}