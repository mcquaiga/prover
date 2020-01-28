using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;

namespace Devices.Honeywell.Core.Repository
{
    public class HoneywellDeviceRepository
    {
        #region Constructors

        public HoneywellDeviceRepository(IDeviceTypeDataSource<HoneywellDeviceType> deviceTypeDataSource)
        {
            _deviceTypeDataSource = deviceTypeDataSource;
        }

        #endregion

        #region Fields

        private readonly HashSet<HoneywellDeviceType> _deviceCache = new HashSet<HoneywellDeviceType>();

        private readonly IDeviceTypeDataSource<HoneywellDeviceType> _deviceTypeDataSource;

        private readonly ConcurrentDictionary<string, HashSet<ItemMetadata>> _itemsCache = new
            ConcurrentDictionary<string, HashSet<ItemMetadata>>();

        #endregion

        #region Methods

        public async Task<IEnumerable<HoneywellDeviceType>> GetAll(bool fromCache = true)
        {
            if (!fromCache
                || _deviceCache.Count == 0)
                _deviceCache.UnionWith(await _deviceTypeDataSource.GetDeviceTypes()
                    .ToList().RunAsync(new CancellationToken()));

            return _deviceCache;
        }

        public async Task<HoneywellDeviceType> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name,
                StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}