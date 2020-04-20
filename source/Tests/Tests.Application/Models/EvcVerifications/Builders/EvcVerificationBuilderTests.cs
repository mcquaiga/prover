using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Application.Models.EvcVerifications.Builders;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Prover.Shared.SampleData;
using Prover.Storage.LiteDb;
using Tests.Application.Services;
using Tests.Shared;

namespace Prover.Application.Models.EvcVerifications.Builders.Tests
{
    [TestClass()]
    public class EvcVerificationBuilderTests
    {
        private static DeviceInstance _device;
        private static DeviceType _deviceType;
        private CompositeDisposable _cleanup;
        private static IDeviceRepository _repo;

        [TestInitialize]
        public async Task Init()
        {
            _cleanup = new CompositeDisposable();
            _device = _deviceType.CreateInstance(SampleItemFiles.MiniMaxItemFile);

            await Task.CompletedTask;
        }

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            _repo = StorageTestsInitialize.DeviceRepo;
            _deviceType = _repo.GetByName("Mini-Max");

            await Task.CompletedTask;
        }

        [TestMethod()]
        public void EvcVerificationBuilderTest()
        {
           
        }

        [TestMethod()]
        public void CreateNewTest()
        {
            

            var tempTest = VerificationBuilder.CreateNew(_device)
                                              .AddTestPoint(tp => tp.WithTemperature(32m, SampleItemFiles.TempLowItems)
                                                                    .WithVolume())
                                              .AddTestPoint(tp => tp.WithTemperature(60m, SampleItemFiles.TempMidItems))
                                              .AddTestPoint(tp => tp.WithTemperature(90m, SampleItemFiles.TempHighItems))
                                              .Build();

            var ptzTest2 = VerificationBuilder.CreateNew(_device)
                                              .AddTestPoint(ct => ct.WithPtz(32m, 80m, 14.73m).WithVolume())
                                              .AddTestPoint(ct => ct.WithPtz(60m, 50m, 14.73m))
                                              .AddTestPoint(ct => ct.WithPtz(90m, 20m, 14.73m))
                                              .Build();

            var ptz = VerificationBuilder.CreateNew(_device)
                                         .AddTestPoint(tp => tp.WithTemperature(32m, SampleItemFiles.TempLowItems)
                                                               .WithPressure(80m, 14.73m, SampleItemFiles.PressureTest(0))
                                                               .WithSuperFactor()
                                                               .WithVolume())
                                         .Build();
        }

        [TestMethod()]
        public void GetEvcVerificationTest()
        {
           
        }

        [TestMethod()]
        public void SetTestDateTimeTest()
        {
           
        }

        [TestMethod()]
        public void TestPointFactoryTest()
        {
           
        }
    }
}