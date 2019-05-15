using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core
{
    public class HoneywellDevice : HoneywellDeviceType, IDeviceWithValues
    {
        public CompositionType Composition
        {
            get
            {
                if (SiteInfo.PressureFactor == CorrectionFactor.Live && SiteInfo.TemperatureFactor == CorrectionFactor.Live)
                    return CompositionType.PressureTemperature;

                if (SiteInfo.PressureFactor == CorrectionFactor.Live)
                    return CompositionType.PressureOnly;

                if (SiteInfo.TemperatureFactor == CorrectionFactor.Live)
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

        public HoneywellDevice(HoneywellDeviceType evcType, IEnumerable<ItemValue> itemValues) : base(evcType.Items)
        {
            ItemValues = itemValues;
            DeviceType = evcType;

            AccessCode = evcType.AccessCode;
            Id = evcType.Id;
            CanUseIrDaPort = evcType.CanUseIrDaPort;
            IsHidden = evcType.IsHidden;
            MaxBaudRate = evcType.MaxBaudRate;
            Name = evcType.Name;

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

        public override T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values)
        {
            var results = values.Union(ItemValues, new ItemValueComparer());

            return base.GetItemValuesByGroup<T>(results);
        }

        protected readonly HoneywellDeviceType DeviceType;
    }
}