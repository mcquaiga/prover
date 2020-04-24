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
using Prover.DevTools.SampleData;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Tests.Application.ExternalDevices.DInOutBoards;
using Tests.Application.Services;
using Tests.Shared;

namespace Prover.Application.Services.Tests
{
    [TestClass()]
    public class VerificationTestServiceTests
    {
        private static Mock<IAsyncRepository<EvcVerificationTest>> _verificationRepository;
        private static VerificationCache _cache;

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

            _cache = new VerificationCache(_verificationRepository.Object, mainScheduler: scheduler.TaskPool);

            //scheduler.TaskPool.Schedule(() => _verificationRepository.Object.UpsertAsync(_device.NewVerification()));

            var shared = _cache.Items.Connect();

            shared
                   .QueryWhenChanged(query => query.Count)
                   .Do(x => Debug.WriteLine($"{x}"))
                   //.ObserveOn(scheduler.Dispatcher)
                   .Subscribe();

            shared.QueryWhenChanged(query =>
            {
                return query.Items.Count();
            })
             //  .LogDebug("shared 2 change")

             //.AutoRefreshOnObservable(x => shared.WhereReasonsAre(ChangeReason.Add).CountChanged().Select(_ => Unit.Default))
             //.ToCollection()
             .ObserveOn(dispatcher)
             //.Select(x => x.Count)
             //.Do(x => Debug.WriteLine($"{x}"))
             .LogDebug(x => $"totals changed {x}")
            .Subscribe();

            // scheduler.TaskPool.Start();

            while (true)
            {
                scheduler.TaskPool.AdvanceBySeconds(5);
                scheduler.Dispatcher.AdvanceByMilliSeconds(2);
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _tests = new List<EvcVerificationTest>();

            _verificationRepository = new Mock<IAsyncRepository<EvcVerificationTest>>(MockBehavior.Loose);

            _verificationRepository.Setup(repo => repo.Query(null)).ReturnsAsync(_tests);
            _verificationRepository.Setup(repo => repo.UpsertAsync(It.IsAny<EvcVerificationTest>()))
                                   .Returns<EvcVerificationTest>(async test =>
                                   {
                                       await VerificationEvents.OnSave.Publish(test);
                                       _tests.Add(test);
                                       return await Task.FromResult(test);
                                   });


        }
    }
}