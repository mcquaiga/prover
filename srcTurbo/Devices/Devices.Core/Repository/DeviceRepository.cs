using Devices.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Devices.Core.Repository
{
    public class DeviceRepository
    {
        public Dictionary<Guid, IDeviceType> Devices => _devicesDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public DeviceRepository(IEnumerable<IDeviceTypeDataSource<IDeviceType>> deviceDataSource)
        {
            _deviceDataSources.UnionWith(deviceDataSource);
        }

        public DeviceRepository(IDeviceTypeDataSource<IDeviceType> deviceTypeDataSource)
        {
            _deviceDataSources.Add(deviceTypeDataSource);
        }

        public async Task<T> Find<T>(Func<T, bool> predicate, bool fromCache = true) where T : class, IDeviceType
        {
            var results = await GetAll<T>(fromCache);

            return results.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<IDeviceType>> GetAll(bool fromCache = true)
        {
            await GetAllDevicesAsync(_deviceDataSources, fromCache);
            return _deviceCache;
        }

        public async Task<IEnumerable<T>> GetAll<T>(bool fromCache = true) where T : class, IDeviceType
        {
            await GetAllDevicesAsync(FilterDataSourceTypes<T>(), fromCache);
            return FilterCacheByType<T>();
        }

        public async Task<IDeviceType> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private readonly HashSet<IDeviceType> _deviceCache = new HashSet<IDeviceType>();

        private readonly HashSet<IDeviceTypeDataSource<IDeviceType>> _deviceDataSources
            = new HashSet<IDeviceTypeDataSource<IDeviceType>>();

        private readonly ConcurrentDictionary<Guid, IDeviceType> _devicesDict = new ConcurrentDictionary<Guid, IDeviceType>();

        private IEnumerable<T> FilterCacheByType<T>() where T : class, IDeviceType
        {
            var results = _deviceCache.Where(d => typeof(T).IsAssignableFrom(d.GetType()))
                            .Select(t => (T)t);

            return results;
        }

        private IEnumerable<IDeviceTypeDataSource<IDeviceType>> FilterDataSourceTypes<TDevice>() where TDevice : IDeviceType
        {
            return _deviceDataSources.Where(
                s => s.GetType()
                        .GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDeviceTypeDataSource<>))
                        .Any(t => t.GetGenericArguments().Any(g => g == typeof(TDevice))));
        }

        private async Task GetAllDevicesAsync<T>(IEnumerable<IDeviceTypeDataSource<T>> dataSources, bool fromCache = true) where T : class, IDeviceType
        {
            if (!fromCache || _deviceCache.Count == 0)
            {
                foreach (var ds in dataSources)
                {
                    await ds.GetDeviceTypes()
                        .ForEachAsync(d =>
                        {
                            _devicesDict.GetOrAdd(Guid.NewGuid(), d);
                        });
                }
                _deviceCache.UnionWith(_devicesDict.Select(kvp => kvp.Value));
            }
        }
    }
}