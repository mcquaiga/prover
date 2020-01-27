using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core
{
    public class HoneywellDeviceInstance : HoneywellDeviceType, IDeviceInstance
    {
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

        public IPressureItems Pressure { get; }

        public ISiteInformationItems SiteInfo { get; }

        public ISuperFactorItems SuperFactor { get; }

        public ITemperatureItems Temperature { get; }

        public IVolumeItems Volume { get; }

        public HoneywellDeviceInstance(HoneywellDeviceType evcType, IEnumerable<ItemValue> itemValues) : base(evcType.Items)
        {
            ItemValues = itemValues;

            SiteInfo = GetItemValuesByGroup<ISiteInformationItems>();
            Pressure = GetItemValuesByGroup<IPressureItems>();
            Temperature = GetItemValuesByGroup<ITemperatureItems>();
            SuperFactor = GetItemValuesByGroup<ISuperFactorItems>();
            Volume = GetItemValuesByGroup<IVolumeItems>();
        }

        public T GetItemValuesByGroup<T>() where T : IItemsGroup
        {
            return GetItemValuesByGroup<T>(ItemValues);
        }

        public T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup
        {
            var results = values.Union(ItemValues, new ItemValueComparer());

            return GetItemValuesByGroup<T>();
        }
    }
}