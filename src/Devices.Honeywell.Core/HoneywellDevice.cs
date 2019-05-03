using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Honeywell.Core
{
    public class HoneywellDevice : IDevice
    {
        private readonly Dictionary<string, string> _items;

        public HoneywellDevice(IDeviceType evcType, Dictionary<string, string> items)
        {
            Type = evcType;
            _items = items;
        }

        public CompositionType CompositionType
        {
            get
            {
                if (string.Equals(ItemValues.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(ItemValues.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return CompositionType.PressureTemperature;

                if (string.Equals(ItemValues.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return CompositionType.PressureOnly;

                if (string.Equals(ItemValues.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return CompositionType.TemperatureOnly;

                return CompositionType.TemperatureOnly;
            }
        }

        public IEnumerable<ItemValue> ItemValues { get; set; }
        public IPressureItems PressureItems { get; set; }
        public ISiteInformationItems SiteInformationItems { get; set; }
        public ISuperFactorItems SuperFactorItems { get; set; }
        public ITemperatureItems TemperatureItems { get; set; }
        public IDeviceType Type { get; set; }
        public IVolumeItems VolumeItems { get; set; }

        public Task GetAllItems()
        {
            throw new NotImplementedException();
        }

        public TValue GetItemValue<TValue>(string code) where TValue : struct
        {
            throw new NotImplementedException();
        }

        public ItemValue GetItemValue(string code)
        {
            throw new NotImplementedException();
        }

        public Task<IPressureItems> GetPressureItems()
        {
            throw new NotImplementedException();
        }

        public IPressureItems GetPressureItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }

        public Task<ITemperatureItems> GetTemperatureItems()
        {
            throw new NotImplementedException();
        }

        public ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }

        public TValue GetValue<TValue>(string code) where TValue : struct
        {
            throw new NotImplementedException();
        }

        public Task<IVolumeItems> GetVolumeItems()
        {
            throw new NotImplementedException();
        }

        public IVolumeItems GetVolumeItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }
    }
}