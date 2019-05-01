using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Devices.Honeywell.Core
{
    [TestClass]
    public class JsonDeviceDataSourceFromItemFileTests
    {
        #region Fields

        private Mock<IStreamReader> _streamMock = new Mock<IStreamReader>();

        #endregion

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public async Task LoadDeviceSuccessfulTest()
        {
            var ds = new JsonDeviceDataSource(new FileStreamReader());

            var devices = new ConcurrentBag<IHoneywellDeviceType>();

            await ds.GetDeviceTypes()
                .ForEachAsync(d =>
                {
                    Assert.IsNotNull(d);
                    Assert.IsNotNull(d.CanUseIrDaPort);
                    Assert.IsTrue(d.Id != 0);

                    Assert.IsFalse(d.Definitions
                        .GroupBy(n => n.Number)
                        .Any(c => c.Count() > 1));
                    devices.Add(d);
                });

            Assert.IsTrue(devices.Count == 6);
        }

        [TestMethod]
        public async Task ParseOneItemTest()
        {
            var ds = new JsonDeviceDataSource(new FileStreamReader());

            var itemsBag = new ConcurrentBag<ItemMetadata>();

            var obs = ds.GetItems()
                .Do(x => Console.WriteLine($"Master {x.Number} from thread: {Thread.CurrentThread.ManagedThreadId}"));

            var items = await obs.SubscribeOn(Scheduler.NewThread)
                .ToList()
                .Do(x => Console.WriteLine($"List count {x.Count} from thread: {Thread.CurrentThread.ManagedThreadId}"));

            await obs
                .SubscribeOn(Scheduler.NewThread)
                .ForEachAsync(i =>
                {
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.ItemDescriptions);
                    itemsBag.Add(i);
                    Console.WriteLine($"Slave Item {i.Number} from thread: {Thread.CurrentThread.ManagedThreadId}");
                });

            Assert.IsTrue(items.Count == itemsBag.Count);
        }

        #endregion
    }
}