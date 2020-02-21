using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core.Devices
{
    public class HoneywellDeviceBuilder : DeviceBuilder
    {
        public HoneywellDeviceBuilder(HoneywellDeviceType deviceType, IEnumerable<ItemValue> itemValues) : base(
            deviceType)
        {
            SetItemValues(itemValues);
        }

        public DeviceBuilder BuildDriveType()
        {
            //var volume = DeviceInstance.ItemGroup<VolumeItems>();

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

        public DeviceBuilder BuildPtz()
        {
            //var site = DeviceInstance.GetItemsByGroup<ISiteInformationItems>();
            //if (site.PressureFactorLive == CorrectionFactorType.Live)
            //    AddItemGroupToAttributes<IPressureItems>();

            return this;
        }

        //private void AddItemGroupToAttributes<T>() where T : ItemGroup
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