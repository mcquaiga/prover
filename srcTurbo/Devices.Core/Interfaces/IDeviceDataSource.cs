using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDeviceDataSource<out TDevice>
        where TDevice : IDeviceType
    {
        IObservable<TDevice> GetDeviceTypes();

        Task<IEnumerable<ItemMetadata.ItemDescription>> GetItemDescriptionsAsync(string name);

        IObservable<ItemMetadata> GetItems();
    }
}