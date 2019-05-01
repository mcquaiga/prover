using Devices.Core.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface IDeviceDataSource<out TDevice>
        where TDevice : IDeviceType
    {
        #region Methods

        IObservable<TDevice> GetDeviceTypes();

        Task<IEnumerable<ItemMetadata.ItemDescription>> GetItemDescriptions(string name);

        IObservable<ItemMetadata> GetItems();

        #endregion
    }
}