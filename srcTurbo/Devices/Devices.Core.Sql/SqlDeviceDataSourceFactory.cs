using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;

namespace Devices.Core.Database
{
    public class SqlDeviceTypeDataSource : IDeviceTypeDataSource<IDeviceType>
    {

        public SqlDeviceTypeDataSource()
        {

        }

        public IObservable<IDeviceType> GetDeviceTypes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemMetadata.ItemDescription>> GetItemDescriptionsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public IObservable<ItemMetadata> GetItems()
        {
            throw new NotImplementedException();
        }
    }
}