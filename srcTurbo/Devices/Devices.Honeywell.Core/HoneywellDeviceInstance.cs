using System.Collections.Generic;
using System.Linq;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items.ItemGroups;

namespace Devices.Honeywell.Core
{
    public interface IHoneywellDeviceInstance : IDeviceInstance
    {
  
    }

    public class HoneywellDeviceInstance : IHoneywellDeviceInstance
    {
        public IDeviceType DeviceType { get; set; }

        public CompositionType Composition
        {
            get
            {
                if (SiteInfo.PressureFactor == CorrectionFactorType.Live && SiteInfo.TemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.PressureTemperature;

                if (SiteInfo.PressureFactor == CorrectionFactorType.Live)
                    return CompositionType.PressureOnly;

                if (SiteInfo.TemperatureFactor == CorrectionFactorType.Live)
                    return CompositionType.TemperatureOnly;

                return CompositionType.PressureTemperature;
            }
        }

        public IEnumerable<ItemValue> ItemValues { get; set; }

        public IPressureItems Pressure { get; set; }

        public ISiteInformationItems SiteInfo { get; set; }

        public ISuperFactorItems SuperFactor { get; set; }

        public ITemperatureItems Temperature { get; set; }

        public IVolumeItems Volume { get; set; }

        public T GetItemsByGroup<T>() where T : IItemsGroup
        {
            return ItemGroupHelpers.GetItemGroup<T>();
        }

        public T GetItemsByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup
        {
            return ItemGroupHelpers.GetItemGroup<T>(values);
        }

        internal void SetItemValueGroups(IEnumerable<ItemValue> itemValues)
        {
            ItemValues = itemValues.ToList();

            SiteInfo = ItemGroupHelpers.GetItemGroup<ISiteInformationItems>(ItemValues);
            Pressure = ItemGroupHelpers.GetItemGroup<IPressureItems>(ItemValues);
            Temperature = ItemGroupHelpers.GetItemGroup<ITemperatureItems>(ItemValues);
            SuperFactor = ItemGroupHelpers.GetItemGroup<ISuperFactorItems>(ItemValues);
            Volume = ItemGroupHelpers.GetItemGroup<IVolumeItems>(ItemValues);
        }
    }
}