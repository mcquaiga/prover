using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using System.Collections.Generic;
using Devices.Core.Items.ItemGroups;

namespace Devices.Core.Interfaces
{
    public interface IDeviceInstance
    {
        IDeviceType DeviceType { get; set; }

        CompositionType Composition { get; }

        IEnumerable<ItemValue> ItemValues { get; set; }

        IPressureItems Pressure { get; }

        ISiteInformationItems SiteInfo { get; }

        ISuperFactorItems SuperFactor { get; }

        ITemperatureItems Temperature { get; }

        IVolumeItems Volume { get; }

        T GetItemsByGroup<T>() where T : IItemsGroup;

        T GetItemsByGroup<T>(IEnumerable<ItemValue> values) where T : IItemsGroup;
    }

    //public interface IMechanicalDeviceInstance : IDeviceInstance<T>
    //    where T : IDeviceType
    //{
    //    IEnergyItems Energy { get; set; }
    //}
}