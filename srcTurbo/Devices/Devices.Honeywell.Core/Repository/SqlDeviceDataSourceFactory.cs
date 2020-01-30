using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Items;
using Devices.Core.Repository;

namespace Devices.Honeywell.Core.Repository
{
    public class SqlDeviceDataSourceFactory : IDeviceTypeDataSource<IHoneywellDeviceType>
    {
        public static IDeviceTypeDataSource<IDeviceType> Instance => _lazy.Value;

        public static IDeviceTypeDataSource<IDeviceType> GetInstance(string directory)
        {
            _directory = directory;
            return _lazy.Value;
        }

        private static readonly Lazy<IDeviceTypeDataSource<IDeviceType>> _lazy
                    = new Lazy<IDeviceTypeDataSource<IDeviceType>>(() => Factory());

        private static string _directory;

        private static IDeviceTypeDataSource<IDeviceType> Factory()
        {
            return new JsonDeviceTypeDataSource(new FileStreamReader(_directory));
        }

        public IObservable<IHoneywellDeviceType> GetDeviceTypes()
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