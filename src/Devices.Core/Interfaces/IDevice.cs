using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDevice
    {
        CompositionType CompositionType { get; }

        IEnumerable<ItemValue> ItemValues { get; set; }
        IPressureItems PressureItems { get; }
        ISiteInformationItems SiteInformationItems { get; }
        ISuperFactorItems SuperFactorItems { get; }
        ITemperatureItems TemperatureItems { get; }
        IDeviceType Type { get; set; }
        IVolumeItems VolumeItems { get; }

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
    }
}