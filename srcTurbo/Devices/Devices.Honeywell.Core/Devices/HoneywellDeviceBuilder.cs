using System;
using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;

namespace Devices.Honeywell.Core.Devices
{
    public class HoneywellDeviceBuilder : DeviceBuilder
    {
        public HoneywellDeviceBuilder(HoneywellDeviceType deviceType, IEnumerable<ItemValue> itemValues) : base(
            deviceType)
        {
            SetItemValues(itemValues);
        }

        public override DeviceBuilder BuildDriveType()
        {
            //var volume = DeviceInstance.ItemGroup<IVolumeItems>();

            //if (volume.DriveRateDescription.Equals("Rotary"))
            //{
            //    DeviceInstance.AddAttribute(DeviceInstance.GetItemsByGroup<RotaryMeterItems>());
            //}
            //else
            //{
            //    DeviceInstance.AddAttribute(DeviceInstance.GetItemsByGroup<EnergyItems>());
            //}

            return this;
        }

        public override DeviceBuilder BuildPtz()
        {
            //var site = DeviceInstance.GetItemsByGroup<ISiteInformationItems>();
            //if (site.PressureFactorLive == CorrectionFactorType.Live)
            //    AddItemGroupToAttributes<IPressureItems>();

            return this;
        }

        //private void AddItemGroupToAttributes<T>() where T : IItemGroup
        //{
        //    DeviceInstance.AddAttribute(DeviceInstance.<T>());
        //}

        public sealed override DeviceBuilder SetItemValues(IEnumerable<ItemValue> values)
        {
            DeviceInstance.SetItemValues(values);
            return this;
        }

    }
}