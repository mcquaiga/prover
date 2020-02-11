using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.Honeywell.Core.Repository
{
    [TestClass]
    public class JsonDeviceDataSourceFromItemFileTests
    {
        private Mock<IStreamReader> _streamMock = new Mock<IStreamReader>();

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public async Task LoadDeviceSuccessfulTest()
        {
            var ds = MiJsonDeviceTypeDataSource.Instance;

            var devices = new ConcurrentBag<HoneywellDeviceType>();

            await ds.GetDeviceTypes()
                .ForEachAsync(d =>
                {
                    Assert.IsNotNull(d);
                    Assert.IsNotNull(d.CanUseIrDaPort);
                    Assert.IsTrue(d.Id != Guid.Empty);

                    Assert.IsFalse(d.Items
                        .GroupBy(n => n.Number)
                        .Any(c => c.Count() > 1));
                    devices.Add(d);
                });

            Assert.IsTrue(devices.Count == 6);
        }

        [TestMethod]
        public async Task ParseOneItemTest()
        {
            var ds = MiJsonDeviceTypeDataSource.Instance;

            var itemsBag = new ConcurrentBag<ItemMetadata>();

            var obs = ds.GetItems()
                .Do(x => Console.WriteLine($"Master {x.Number} from thread: {Thread.CurrentThread.ManagedThreadId}"));

            var items = await obs.SubscribeOn(NewThreadScheduler.Default)
                .ToList()
                .Do(x => Console.WriteLine($"List count {x.Count} from thread: {Thread.CurrentThread.ManagedThreadId}"));

            await obs
                .SubscribeOn(NewThreadScheduler.Default)
                .ForEachAsync(i =>
                {
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.ItemDescriptions);
                    itemsBag.Add(i);
                    Console.WriteLine($"Slave Item {i.Number} from thread: {Thread.CurrentThread.ManagedThreadId}");
                });

            Assert.IsTrue(items.Count == itemsBag.Count);
        }
    }
}