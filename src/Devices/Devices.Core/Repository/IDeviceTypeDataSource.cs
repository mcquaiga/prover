using System;
using Devices.Core.Interfaces;
using Devices.Core.Items;

namespace Devices.Core.Repository
{
    public interface IDeviceTypeDataSource<out TDevice>
        where TDevice : DeviceType
    {

        IObservable<TDevice> GetDeviceTypes();


        IObservable<ItemMetadata> GetItems();


    }
}