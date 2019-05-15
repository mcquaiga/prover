//using Devices.Core.Interfaces;
//using Devices.Core.Items;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Devices.Honeywell.Core.Repository
//{
//    public class HoneywellDeviceRepository
//    {
//        #region Fields

// private readonly HashSet<HoneywellDeviceType> _deviceCache = new HashSet<HoneywellDeviceType>();

// private readonly IDeviceDataSource<HoneywellDeviceType> _deviceDataSource;

// private readonly ConcurrentDictionary<string, HashSet<ItemMetadata>> _itemsCache = new
// ConcurrentDictionary<string, HashSet<ItemMetadata>>();

// #endregion

// #region Constructors

// public HoneywellDeviceRepository(IDeviceDataSource<HoneywellDeviceType> deviceDataSource) {
// _deviceDataSource = deviceDataSource; }

// #endregion

// #region Methods

// public async Task<IEnumerable<HoneywellDeviceType>> GetAll(bool fromCache = true) { if (!fromCache
// || _deviceCache.Count == 0) { _deviceCache.UnionWith( await _deviceDataSource.GetDeviceTypes()
// .ToList() .RunAsync(new CancellationToken())); }

// return _deviceCache; }

// public async Task<HoneywellDeviceType> GetByName(string name, bool fromCache = true) { await GetAll(fromCache);

// return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name,
// StringComparison.OrdinalIgnoreCase)); }

//        #endregion
//    }
//}