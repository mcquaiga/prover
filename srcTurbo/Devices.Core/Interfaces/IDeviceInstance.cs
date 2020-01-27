using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDeviceInstance : IDeviceType
    {
        CompositionType Composition { get; }

        IEnumerable<ItemValue> ItemValues { get; set; }

        IPressureItems Pressure { get; }

        ISiteInformationItems SiteInfo { get; }

        ISuperFactorItems SuperFactor { get; }

        ITemperatureItems Temperature { get; }

        IVolumeItems Volume { get; }

        T GetItemValuesByGroup<T>() where T : IItemsGroup;

        T GetItemValuesByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup;
    }

    public interface IMechanicalDeviceInstance : IDeviceInstance
    {
        IEnergyItems Energy { get; set; }
    }
}