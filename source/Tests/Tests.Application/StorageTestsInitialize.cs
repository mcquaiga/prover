using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Caching;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Factories;
using Prover.Storage.LiteDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Verifications.Factories;

namespace Tests.Application.Services
{
	[TestClass]
	public class StorageTestsInitialize
	{
		//public static VerificationService ViewModelService { get; private set; }
		public static ICacheClient<EvcVerificationTest> VerificationCache { get; private set; }
		public static Mock<IVerificationManagerService> VerificationManager = new Mock<IVerificationManagerService>();

		public static VerificationsLiteDbRepository TestRepo { get; private set; }

		public static IDeviceRepository DeviceRepo { get; private set; }

		public static ILogger Logger { get; } = new DebugLoggerProvider().CreateLogger("StorageTests");
		public static ILoggerProvider LoggerProvider { get; } = new DebugLoggerProvider();
		public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();


		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			Console.WriteLine("AssemblyCleanup");
		}

		[AssemblyInitialize]
		public static async Task AssemblyInitialize(TestContext context)
		{
			DeviceRepo = DeviceRepository.Instance;

			LiteDbStorageDefaults.CreateLiteDb();
			TestRepo = new VerificationsLiteDbRepository(LiteDbStorageDefaults.Database);
			//ViewModelService = CreateVerificationTestService();
			Console.WriteLine("AssemblyInitialize");
			VerificationCache = new EntityCache<EvcVerificationTest>(LoggerFactory.CreateLogger<EntityCache<EvcVerificationTest>>());

		}

		public static async Task<EvcVerificationViewModel> CreateAndSaveNewTest(DeviceInstance device)
		{
			var newTest = device.NewVerification();
			newTest = await TestRepo.UpsertAsync(newTest);
			return newTest.ToViewModel();
		}

		public static async Task<EvcVerificationViewModel> CreateAndSaveNewTest(DeviceType deviceType,
			Dictionary<int, string> itemValues, IVerificationService testService = null) =>
			await CreateAndSaveNewTest(deviceType.CreateInstance(itemValues));

		public static async Task<ICollection<EvcVerificationViewModel>> CreateAndSaveNewTests(DeviceType deviceType,
			Dictionary<int, string> itemValues, int numberOfTest, IVerificationService testService = null)
		{
			var records = new List<EvcVerificationViewModel>();
			var tasks = new List<Task<EvcVerificationViewModel>>();
			for (var i = 0; i < numberOfTest; i++)
			{
				var device = deviceType.CreateInstance(itemValues);
				records.Add(await CreateAndSaveNewTest(device));
			}

			return records;
		}

		public static VerificationService CreateVerificationTestService()
		{


			return new VerificationService(logger: LoggerFactory.CreateLogger<VerificationService>(), deviceRepository: DeviceRepo, verificationRepository: TestRepo,
			managerService: VerificationManager.Object, deviceManager: null);
		}


		public static void DropCollection()
		{
			LiteDbStorageDefaults.Database.DropCollection("EvcVerificationTest");
		}
	}
}