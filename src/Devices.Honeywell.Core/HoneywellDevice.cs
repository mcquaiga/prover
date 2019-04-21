using Devices.Core.EvcDevices;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Honeywell.Core
{
    public class HoneywellDevice : IEvcDevice
    {
        #region Constructors

        public HoneywellDevice(IEvcDeviceType evcType, Dictionary<string, string> items)
        {
            Device = evcType;
        }

        #endregion

        #region Properties

        public EvcCorrectorType CompositionType => throw new NotImplementedException();

        public EvcCorrectorType CorrectorType
        {
            get
            {
                if (string.Equals(ItemValues.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(ItemValues.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return EvcCorrectorType.PTZ;

                if (string.Equals(ItemValues.GetItem(ItemCodes.Pressure.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return EvcCorrectorType.P;

                if (string.Equals(ItemValues.GetItem(ItemCodes.Temperature.FixedFactor).Description, "live", StringComparison.OrdinalIgnoreCase))
                    return EvcCorrectorType.T;

                return EvcCorrectorType.T;
            }
        }

        public IEvcDeviceType Device { get; set; }

        public IFrequencyTestItems FrequencyItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool HasFrequency { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<ItemValue> ItemValues { get; set; }

        public IPressureItems PressureItems { get; set; }

        public ISiteInformationItems SiteInformationItems { get; set; }

        public ISuperFactorItems SuperFactorItems { get; set; }

        public ITemperatureItems TemperatureItems { get; set; }

        public IVolumeItems VolumeItems { get; set; }

        #endregion

        #region Methods

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

        #endregion
    }
}