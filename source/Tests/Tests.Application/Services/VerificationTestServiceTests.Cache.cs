using Devices.Core.Interfaces;
using Devices.Core.Items;
using DynamicData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prover.Application.Caching;
using Prover.Application.Extensions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.Verifications;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Modules.DevTools.SampleData;
using Tests.Application.Services;
using Tests.Shared;

namespace Prover.Application.Services.Tests
{
	[TestClass()]
	public class VerificationTestServiceTests
	{
		private static Mock<IAsyncRepository<EvcVerificationTest>> _verificationRepository;
		private static ICacheClient<EvcVerificationTest> _cache;

		private static List<EvcVerificationTest> _tests;
		private DeviceType _deviceType = StorageTestsInitialize.DeviceRepo.GetByName("Mini-Max");
		private DeviceInstance _device;

		[TestInitialize]
		public async Task Init()
		{
			_tests.Clear();

			_device = _deviceType.CreateInstance(SampleItemFiles.MiniMaxItemFile);



		}

		[TestMethod()]
		public void ApplyDateFilterTest()
		{
			var scheduler = new TestSchedulers();
			var dispatcher = scheduler.Dispatcher;

			_tests.Add(_device.NewVerification());

			_cache = new EntityCache<EvcVerificationTest>(null, scheduler: scheduler.TaskPool);

			//scheduler.TaskPool.Schedule(() => _verificationRepository.Object.UpsertAsync(_device.NewVerification()));

			var shared = _cache.Data.Connect();

			shared.QueryWhenChanged(query => query.Count)
				  .Do(x => Debug.WriteLine($"{x}"))
				  .Subscribe();

			shared.QueryWhenChanged(query => query.Items.Count())
				  .ObserveOn(dispatcher)
				  .LogDebug(x => $"totals changed {x}")
				  .Subscribe();

			//  .LogDebug("shared 2 change")
			//.Select(x => x.Count)
			//.Do(x => Debug.WriteLine($"{x}"))
			//.AutoRefreshOnObservable(x => shared.WhereReasonsAre(ChangeReason.Add).CountChanged().Select(_ => Unit.Default))
			//.ToCollection()
			// scheduler.TaskPool.Start();

			//while (true)
			//{
			//    scheduler.TaskPool.AdvanceBySeconds(5);
			//    scheduler.Dispatcher.AdvanceByMilliSeconds(2);
			//}
		}

		//[TestMethod()]
		//public void DisposeTest()
		//{
		//    Assert.Fail();
		//}

		//[TestMethod()]
		//public void LoadAsyncTest()
		//{
		//    Assert.Fail();
		//}

		//[TestMethod()]
		//public void UpdateTest()
		//{
		//    Assert.Fail();
		//}

		[ClassInitialize]
		public static async Task ClassInitialize(TestContext context)
		{
			_tests = new List<EvcVerificationTest>();
			_verificationRepository = GetVerificationRepostiory(_tests);
		}

		private static Mock<IAsyncRepository<EvcVerificationTest>> GetVerificationRepostiory(ICollection<EvcVerificationTest> mockedItems = null)
		{
			mockedItems = mockedItems ?? new List<EvcVerificationTest>();

			var repo = new Mock<IAsyncRepository<EvcVerificationTest>>(MockBehavior.Loose);

			repo.Setup(repo => repo.QueryAsync(It.IsAny<IQuerySpecification<EvcVerificationTest>>()))
				// .ReturnsAsync<IQuerySpecification<EvcVerificationTest>, IEnumerable<EvcVerificationTest>>(spec => mockedItems.Where(spec))
				.ReturnsAsync(mockedItems);

			//repo.Setup(repo => repo.Query(It.IsAny<Expression<Func<EvcVerificationTest, bool>>>()))
			//	.ReturnsAsync(mockedItems);

			repo.Setup(repo => repo.UpsertAsync(It.IsAny<EvcVerificationTest>()))
				.Returns<EvcVerificationTest>(async test =>
				{
					await VerificationEvents.OnSave.Publish(test);
					mockedItems.Add(test);
					return await Task.FromResult(test);
				});

			return repo;
		}
	}
}