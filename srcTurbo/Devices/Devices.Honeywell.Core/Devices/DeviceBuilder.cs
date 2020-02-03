using System;
using System.Collections.Generic;
using Devices.Core;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Devices
{
    public abstract class DeviceBuilder
    {
        protected readonly HoneywellDeviceType DeviceType;
        protected HoneywellDeviceInstance DeviceInstance;

        protected DeviceBuilder(HoneywellDeviceType deviceType)
        {
            DeviceType = deviceType;
            DeviceInstance = new HoneywellDeviceInstance(DeviceType);
        }

        #region Public Methods

        public HoneywellDeviceInstance GetDeviceInstance()
        {
            return DeviceInstance;
        }

        public virtual DeviceBuilder Reset()
        {
            DeviceInstance = new HoneywellDeviceInstance(DeviceType);
            return this;
        }

        public abstract DeviceBuilder BuildAttributes();

        public abstract DeviceBuilder BuildDriveType();

        public abstract DeviceBuilder BuildPtz();

        public abstract DeviceBuilder SetItemValues(IEnumerable<ItemValue> values);

        #endregion
    }

    public class HoneywellDeviceBuilder : DeviceBuilder
    {
        public HoneywellDeviceBuilder(HoneywellDeviceType deviceType, IEnumerable<ItemValue> itemValues) : base(
            deviceType)
        {
            SetItemValues(itemValues);
        }

        #region Public Methods

        public override DeviceBuilder BuildAttributes()
        {
            return this;
        }

        public override DeviceBuilder BuildDriveType()
        {
            var volume = DeviceInstance.GetItemsByGroup<IVolumeItems>();

            if (volume.DriveRateDescription.Equals("Rotary"))
            {
                DeviceInstance.AddAttribute(DeviceInstance.GetItemsByGroup<RotaryMeterItems>());
            }
            else
            {
                DeviceInstance.AddAttribute(DeviceInstance.GetItemsByGroup<EnergyItems>());
            }

            return this;
        }

        public override DeviceBuilder BuildPtz()
        {
            var site = DeviceInstance.GetItemsByGroup<ISiteInformationItems>();
            if (site.PressureFactorLive == CorrectionFactorType.Live)
                AddItemGroupToAttributes<IPressureItems>();

            return this;
        }

        private void AddItemGroupToAttributes<T>() where T : IItemGroup
        {
            DeviceInstance.AddAttribute(DeviceInstance.GetItemsByGroup<T>());
        }

        public sealed override DeviceBuilder SetItemValues(IEnumerable<ItemValue> values)
        {
            DeviceInstance.ItemValues.Clear();
            DeviceInstance.ItemValues.UnionWith(values);
            return this;
        }

        #endregion

        #region Private

        private static ItemGroup GetItemGroup(IEnumerable<ItemValue> values, Type groupType)
        {
            //if (!groupType.GetInterfaces().Contains(typeof(IItemGroup)))
            //    throw new Exception($"Type {groupType.Name} must inherit from {typeof(ItemGroup).Name}.");

            var itemType = groupType.GetMatchingItemGroupClass();
            if (itemType == null)
                throw new Exception($"Type {groupType.Name} could not be found.");

            var itemGroup = (ItemGroup) Activator.CreateInstance(itemType);

            if (values == null) values = new List<ItemValue>();

            itemGroup.SetValues(values);

            return itemGroup;
        }

        #endregion
    }

    public class MechanicalDeviceBuilder
    {
    }
}