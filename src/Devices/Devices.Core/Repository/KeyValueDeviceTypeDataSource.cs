using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Shared.Interfaces;

namespace Devices.Core.Repository
{
    public class KeyValueDeviceTypeDataSource : IDeviceTypeCacheSource<DeviceType>
    {
        private readonly IKeyValueStore _keyValueRepository;

        public KeyValueDeviceTypeDataSource(IKeyValueStore keyValueRepository)
        {
            _keyValueRepository = keyValueRepository;
        }

        public IObservable<DeviceType> GetDeviceTypes()
        {
            return _keyValueRepository.GetAll<DeviceType>().ToObservable();
        }

        public IObservable<ItemMetadata> GetItems()
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<DeviceType> deviceTypes)
        {
            deviceTypes.ToList().ForEach(d =>
            {
                _keyValueRepository.AddOrUpdate(d);
            });
        }
    }
}