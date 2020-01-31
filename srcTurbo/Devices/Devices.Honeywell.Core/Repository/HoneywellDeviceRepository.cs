using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;

namespace Devices.Honeywell.Core.Repository
{
    public class HoneywellDeviceRepository
    {
        #region Constructors

        public HoneywellDeviceRepository(IDeviceTypeDataSource<IHoneywellDeviceType> deviceTypeDataSource)
        {
            _deviceTypeDataSource = deviceTypeDataSource;
        }

        #endregion

        #region Fields

        private readonly HashSet<IHoneywellDeviceType> _deviceCache = new HashSet<IHoneywellDeviceType>();

        private readonly IDeviceTypeDataSource<IHoneywellDeviceType> _deviceTypeDataSource;

        private readonly ConcurrentDictionary<string, HashSet<ItemMetadata>> _itemsCache = new
            ConcurrentDictionary<string, HashSet<ItemMetadata>>();

        #endregion

        #region Methods

        public async Task<IEnumerable<IHoneywellDeviceType>> GetAll(bool fromCache = true)
        {
            if (!fromCache || _deviceCache?.Count == 0)
                await LoadDevices();

            return _deviceCache.AsEnumerable();
        }

        private async Task LoadDevices()
        {
            _deviceCache.Clear();

            var devices =
                await _deviceTypeDataSource.GetDeviceTypes()
                    .ToList()
                    .RunAsync(new CancellationToken());

            _deviceCache.UnionWith(devices);
        }

        public async Task<IHoneywellDeviceType> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name,
                StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}