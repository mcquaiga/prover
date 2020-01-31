using System.Collections.Generic;
using System.IO;
using Devices.Core.Items;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace Tests.Honeywell.Core
{
    public abstract class BaseHoneywellTest
    {
        protected Mock<HoneywellDeviceType> Device;
        protected Mock<HoneywellDeviceType> Device2;
        protected List<HoneywellDeviceType> DevicesList = new List<HoneywellDeviceType>();
        public Dictionary<int, string> MiniMaxItemFile => JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText("MiniMax.json"));

        public Dictionary<int, string> MiniMaxPressureItemFile => JsonConvert.DeserializeObject<Dictionary<int, string>>(
            "{'8':'  80.134','44':'  6.4402','47':'  1.0076'}");

        [TestInitialize]
        public virtual void SetupDevice()
        {
            var items = new List<ItemMetadata>(){
                    new ItemMetadata()
                    {
                        Number = 0
                    },
                    new ItemMetadata()
                    {
                        Number = 2
                    }
                };

            Device = new Mock<HoneywellDeviceType>(MockBehavior.Strict, new object[] { items });

            Device.Setup(d => d.Name)
                .Returns("Honeywell");

            Device.Setup(d => d.Id)
                .Returns(3);

            Device.Setup(d => d.AccessCode)
                .Returns("3");

            var items2 = new List<ItemMetadata>(){
                    new ItemMetadata()
                    {
                        Number = 8
                    },
                    new ItemMetadata()
                    {
                        Number = 12
                    }
                };

            Device2 = new Mock<HoneywellDeviceType>(MockBehavior.Strict, new object[] { items2 });

            Device2.Setup(d => d.Name)
               .Returns("Honeywell 2");

            Device2.Setup(d => d.Id)
                .Returns(14);

            Device2.Setup(d => d.AccessCode)
                .Returns("3");

            DevicesList = new List<HoneywellDeviceType>()
            {
                Device.Object,
                Device2.Object
            };
        }
    }
}