using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;
using Devices.Romet.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Tests.Romet.Core
{
    [TestClass]
    public class RometItemGroupTests
    {
        public static Dictionary<int, string> AdemItemFile { get; set; } = new Dictionary<int, string>();
        private static DeviceType _deviceType;
        private static DeviceInstance _instance;
        private static List<ItemValue> _itemValues;

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _deviceType = RometDeviceRepository.GetAdem;
            AdemItemFile = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText("Adem.json"));

            _itemValues = _deviceType.ToItemValuesEnumerable(AdemItemFile).ToList();
        }

        [TestInitialize]
        public void Init()
        {
            _instance = _deviceType.Factory.CreateInstance(_itemValues);
        }

        [TestMethod]
        public void TestCreateDeviceInstance()
        {
            Assert.IsNotNull(_instance);

            var temp = _instance.ItemGroup<TemperatureItems>();
            Assert.IsNotNull(temp);
            Assert.AreEqual(_itemValues.GetItemValue(26), temp.GasTemperature);

            var rotary = _instance.ItemGroup<RotaryMeterItems>();
            Assert.IsNotNull(rotary);
            Assert.IsNotNull(rotary.MeterType);

        }

        [TestMethod]
        public void PressureItemsIsNullTest()
        {
            Assert.IsNull(_instance.ItemGroup<PressureItems>());
        }

        [TestMethod]
        public void CreatesNewObjectTest()
        {
            var expected = 50000m;
            var newInstance = _deviceType.Factory.CreateInstance(_itemValues);
            Assert.AreNotSame(newInstance, _instance);

            Assert.AreNotSame(newInstance.Values, _instance.Values);

            var rotary1 = _instance.ItemGroup<RotaryMeterItems>();
            rotary1.MeterDisplacement = expected;

            var rotary2 = _instance.ItemGroup<RotaryMeterItems>();
            Assert.AreEqual(expected, rotary2.MeterDisplacement);
            Assert.AreSame(rotary1, rotary2);
            var rotary3 = _instance.CreateItemGroup<RotaryMeterItems>();
            Assert.AreNotSame(rotary2, rotary3);
            //Assert.IsNull(_instance.ItemGroup<IPressureItems>());
        }
    }
}
