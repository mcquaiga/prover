using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Devices.Core.Repository
{
    public class DeviceRepository
    {
        #region Fields

        private readonly HashSet<IDeviceType> _deviceCache = new HashSet<IDeviceType>();

        private readonly HashSet<IDeviceDataSource<IDeviceType>> _deviceDataSource
            = new HashSet<IDeviceDataSource<IDeviceType>>();

        #endregion

        #region Constructors

        public DeviceRepository(IEnumerable<IDeviceDataSource<IDeviceType>> deviceDataSource)
        {
            _deviceDataSource.UnionWith(deviceDataSource);
        }

        public DeviceRepository(IDeviceDataSource<IDeviceType> deviceDataSource)
        {
            _deviceDataSource.Add(deviceDataSource);
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<IDeviceType>> GetAll(bool fromCache = true)
        {
            if (!fromCache || _deviceCache.Count == 0)
            {
                foreach (var ds in _deviceDataSource)
                {
                    await ds.GetDeviceTypes()
                        .ForEachAsync(d => _deviceCache.Add(d));
                }
            }

            return _deviceCache;
        }

        public async Task<IDeviceType> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}