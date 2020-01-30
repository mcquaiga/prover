using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;

namespace Devices.Core.Repository
{
    public interface IDeviceTypeDataSource<out TDevice>
        where TDevice : IDeviceType
    {
        IObservable<TDevice> GetDeviceTypes();

        Task<IEnumerable<ItemMetadata.ItemDescription>> GetItemDescriptionsAsync(string name);

        IObservable<ItemMetadata> GetItems();
    }
}