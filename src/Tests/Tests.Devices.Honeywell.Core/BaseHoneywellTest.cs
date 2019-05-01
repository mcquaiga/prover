using Devices.Core.Items;
using Devices.Honeywell.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Tests.Devices.Honeywell.Core
{
    public abstract class BaseHoneywellTest
    {
        #region Methods

        [TestInitialize]
        public virtual void SetupDevice()
        {
            Device = new Mock<IHoneywellDeviceType>();

            Device.Setup(d => d.Name)
                .Returns("Honeywell");

            Device.Setup(d => d.Id)
                .Returns(3);

            Device.Setup(d => d.AccessCode)
                .Returns(3);

            Device.Setup(d => d.Definitions)
                .Returns(new List<ItemMetadata>(){
                    new ItemMetadata()
                    {
                        Number = 0
                    },
                    new ItemMetadata()
                    {
                        Number = 2
                    }
                });

            Device2 = new Mock<IHoneywellDeviceType>();

            Device2.Setup(d => d.Name)
               .Returns("Honeywell 2");

            Device2.Setup(d => d.Id)
                .Returns(14);

            Device2.Setup(d => d.AccessCode)
                .Returns(3);

            Device2.Setup(d => d.Definitions)
                .Returns(new List<ItemMetadata>(){
                    new ItemMetadata()
                    {
                        Number = 8
                    },
                    new ItemMetadata()
                    {
                        Number = 12
                    }
                });

            DevicesList = new List<IHoneywellDeviceType>()
            {
                Device.Object,
                Device2.Object
            };
        }

        #endregion

        #region Fields

        protected Mock<IHoneywellDeviceType> Device;

        protected Mock<IHoneywellDeviceType> Device2;

        protected List<IHoneywellDeviceType> DevicesList = new List<IHoneywellDeviceType>();

        #endregion
    }
}