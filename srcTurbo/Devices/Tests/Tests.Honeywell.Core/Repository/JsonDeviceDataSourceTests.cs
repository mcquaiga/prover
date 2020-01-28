using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Repository;
using ExpectedObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace Tests.Honeywell.Core.Repository
{
    [TestClass]
    public class JsonDeviceDataSourceTests
    {
        private List<ItemMetadata.ItemDescription> _expectedDescriptions;

        private IList<ItemMetadata> _expectedItems;

        private Mock<IStreamReader> _streamMock = new Mock<IStreamReader>();

        [TestMethod]
        public async Task GetItemDescriptionsSuccessfulTest()
        {
            _streamMock.Setup(s => s.GetItemDefinitionsReader("Item_MeterType"))
                          .Returns(ItemTestData.MeterTypeDescriptionsJson.ToStream());

            _streamMock.Setup(s => s.GetItemsReader())
                          .Returns(ItemTestData.MeterTypeJson.ToStream());

            var ds = new JsonDeviceTypeDataSource(_streamMock.Object);

            var actual = await ds.GetItems()
                .ToList()
                .RunAsync(new CancellationToken());

            Assert.IsNotNull(actual.First());
            Assert.IsTrue(actual.First().ItemDescriptions.Count == 2);
            Assert.IsTrue(actual.First().ItemDescriptions.First().GetType() == typeof(MeterIndexItemDescription));
        }

        [TestMethod]
        public async Task GetItemsSuccessfulTest()
        {
            _streamMock.Setup(s => s.GetItemsReader())
                           .Returns(_expectedItems.ToStream());

            var ds = new JsonDeviceTypeDataSource(_streamMock.Object);

            var items = await ds.GetItems()
                .ToList()
                .RunAsync(new CancellationToken());

            Assert.IsTrue(_expectedItems.Count == items.Count);

            _expectedItems.ToExpectedObject()
                .ShouldEqual(items);
        }

        [TestInitialize]
        public void Setup()
        {
            _expectedDescriptions = new List<ItemMetadata.ItemDescription>()
            {
                new ItemMetadata.ItemDescription()
                {
                    Description = "Description 1",
                    Id = 23,
                    NumericValue = 1000
                },
                new ItemMetadata.ItemDescription()
                {
                    Description = "Description 2",
                    Id = 25,
                    NumericValue = 10
                }
            };

            _expectedItems = new List<ItemMetadata>
            {
                new ItemMetadata()
                {
                    Number = 999,
                    Description = "Test Item",
                    Code = "TI",
                    IsPressure = true
                },
                new ItemMetadata(_expectedDescriptions)
                {
                    Number = 555,
                    Description = "Test Item 123",
                    Code = "TI-123",
                    IsPressure = true
                }
            };
        }
    }

    internal static class StreamHelpers
    {
        public static StreamReader ToStream(this ICollection<ItemMetadata> item)
        {
            return JsonConvert.SerializeObject(item).ToStream();
        }

        public static StreamReader ToStream(this string items)
        {
            var byteArray = Encoding.ASCII.GetBytes(items);
            var stream = new MemoryStream(byteArray);

            // convert stream to string
            return new StreamReader(stream);
        }
    }
}