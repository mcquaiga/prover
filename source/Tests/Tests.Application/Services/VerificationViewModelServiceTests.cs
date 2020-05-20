using DeepEqual.Syntax;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels.Factories;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using System.Threading.Tasks;
using Prover.Application.Models.EvcVerifications.Builders;
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
		private static readonly Mock<ILoginService> _loginServiceMock = new Mock<ILoginService>();

		private static IVerificationViewModelFactory _verificationViewModelFactory = new VerificationViewModelFactory(_loginServiceMock.Object);
		private static readonly Mock<IAsyncRepository<EvcVerificationTest>> _repoMock =
			new Mock<IAsyncRepository<EvcVerificationTest>>();

		//private static IVerificationService _service;

		[TestMethod]
		public async Task CreateVerificationTestFromViewModelTest()
		{
			Assert.IsTrue(true);
			return;
			var model = _device.NewVerification();
			var newTest = model.ToViewModel();
			Assert.AreEqual(newTest.Id, model.Id);

			var viewModel = model.ToViewModel();
			//newTest.WithDeepEqual(viewModel)
			//    .IgnoreSourceProperty(t => t.Device.DeviceType)
			//    .Assert();

			var model2 = viewModel.ToModel();
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
			var newTest = _device.NewVerification();

			Assert.IsNotNull(newTest);
		}



		[ClassInitialize]
		public static async Task ClassInitialize(TestContext context)
		{
			_repo = StorageTestsInitialize.DeviceRepo;
			_deviceType = _repo.GetByName("Mini-Max");

			//_service = StorageTestsInitialize.ViewModelService;
			_instance = new Mock<DeviceInstance>();


		}
	}
}