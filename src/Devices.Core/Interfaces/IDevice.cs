using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDevice
    {
        #region Properties

        EvcCorrectorType CompositionType { get; }

        IDeviceType Device { get; set; }

        IEnumerable<ItemValue> ItemValues { get; set; }

        IPressureItems PressureItems { get; set; }

        ISiteInformationItems SiteInformationItems { get; set; }

        ISuperFactorItems SuperFactorItems { get; set; }

        ITemperatureItems TemperatureItems { get; set; }

        IVolumeItems VolumeItems { get; set; }

        #endregion

        #region Methods

        Task GetAllItems();

        TValue GetItemValue<TValue>(string code) where TValue : struct;

        ItemValue GetItemValue(string code);

        Task<IPressureItems> GetPressureItems();

        IPressureItems GetPressureItems(Dictionary<string, string> itemData);

        Task<ITemperatureItems> GetTemperatureItems();

        ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData);

        TValue GetValue<TValue>(string code) where TValue : struct;

        Task<IVolumeItems> GetVolumeItems();

        IVolumeItems GetVolumeItems(Dictionary<string, string> itemData);

        #endregion
    }
}