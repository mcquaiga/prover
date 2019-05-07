using Devices.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devices.Core.Repository
{
    public class DeviceRepository
    {
        public Dictionary<Guid, IDevice> Devices => _devicesDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public DeviceRepository(IEnumerable<IDeviceDataSource<IDevice>> deviceDataSource)
        {
            _deviceDataSources.UnionWith(deviceDataSource);
        }

        public DeviceRepository(IDeviceDataSource<IDevice> deviceDataSource)
        {
            _deviceDataSources.Add(deviceDataSource);
        }

        public async Task<T> Find<T>(Func<T, bool> predicate, bool fromCache = true) where T : class, IDevice
        {
            var results = await GetAll<T>(fromCache);

            return results.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<IDevice>> GetAll(bool fromCache = true)
        {
            await GetAllDevicesAsync(_deviceDataSources, fromCache);
            return _deviceCache;
        }

        public async Task<IEnumerable<T>> GetAll<T>(bool fromCache = true) where T : class, IDevice
        {
            await GetAllDevicesAsync(FilterDataSourceTypes<T>(), fromCache);
            return FilterCacheByType<T>();
        }

        public async Task<IDevice> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private readonly HashSet<IDevice> _deviceCache = new HashSet<IDevice>();

        private readonly HashSet<IDeviceDataSource<IDevice>> _deviceDataSources
            = new HashSet<IDeviceDataSource<IDevice>>();

        private readonly ConcurrentDictionary<Guid, IDevice> _devicesDict = new ConcurrentDictionary<Guid, IDevice>();

        private IEnumerable<T> FilterCacheByType<T>() where T : class, IDevice
        {
            var results = _deviceCache.Where(d => typeof(T).IsAssignableFrom(d.GetType()))
                            .Select(t => (T)t);

            return results;
        }

        private IEnumerable<IDeviceDataSource<IDevice>> FilterDataSourceTypes<TDevice>() where TDevice : IDevice
        {
            return _deviceDataSources.Where(
                s => s.GetType()
                        .GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDeviceDataSource<>))
                        .Any(t => t.GetGenericArguments().Any(g => g == typeof(TDevice))));
        }

        private async Task GetAllDevicesAsync<T>(IEnumerable<IDeviceDataSource<T>> dataSources, bool fromCache = true) where T : class, IDevice
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