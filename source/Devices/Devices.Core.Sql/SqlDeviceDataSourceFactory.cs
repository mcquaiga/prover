using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.Descriptions;
using Devices.Core.Repository;

namespace Devices.Core.Database
{
    public class SqlDeviceTypeDataSource : IDeviceTypeDataSource<DeviceType>
    {

        public SqlDeviceTypeDataSource()
        {

        }

        public IObservable<DeviceType> GetDeviceTypes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemDescription>> GetItemDescriptionsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public IObservable<ItemMetadata> GetItems()
        {
            throw new NotImplementedException();
        }
    }
}