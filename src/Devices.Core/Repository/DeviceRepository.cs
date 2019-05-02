using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devices.Core.Repository
{
    public class DeviceRepository
    {
        private readonly HashSet<IDeviceType> _deviceCache = new HashSet<IDeviceType>();

        private readonly HashSet<IDeviceDataSource<IDeviceType>> _deviceDataSources
            = new HashSet<IDeviceDataSource<IDeviceType>>();

        public DeviceRepository(IEnumerable<IDeviceDataSource<IDeviceType>> deviceDataSource)
        {
            _deviceDataSources.UnionWith(deviceDataSource);
        }

        public DeviceRepository(IDeviceDataSource<IDeviceType> deviceDataSource)
        {
            _deviceDataSources.Add(deviceDataSource);
        }

        public async Task<T> Find<T>(Func<T, bool> predicate, bool fromCache = true) where T : class, IDeviceType
        {
            var results = await GetAll<T>(fromCache);

            return results.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<IDeviceType>> GetAll(bool fromCache = true)
        {
            if (!fromCache || _deviceCache.Count == 0)
            {
                foreach (var ds in _deviceDataSources)
                {
                    await ds.GetDeviceTypes()
                        .ForEachAsync(d => _deviceCache.Add(d));
                }
            }

            return _deviceCache;
        }

        public async Task<IEnumerable<T>> GetAll<T>(bool fromCache = true) where T : class, IDeviceType
        {
            if (!fromCache || _deviceCache.Count == 0)
            {
                foreach (var ds in FilterDataSourceTypes<T>())
                {
                    await ds.GetDeviceTypes()
                        .ForEachAsync(d => _deviceCache.Add(d));
                }
            }

            return FilterCacheByType<T>();
        }

        public async Task<IDeviceType> GetByName(string name, bool fromCache = true)
        {
            await GetAll(fromCache);

            return _deviceCache.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<T> FilterCacheByType<T>() where T : class, IDeviceType
        {
            var results = _deviceCache.Where(d => typeof(T).IsAssignableFrom(d.GetType()))
                            .Select(t => (T)t);

            return results;
        }

        private IEnumerable<IDeviceDataSource<IDeviceType>> FilterDataSourceTypes<TDevice>() where TDevice : IDeviceType
        {
            return _deviceDataSources.Where(
                s => s.GetType()
                        .GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDeviceDataSource<>))
                        .Any(t => t.GetGenericArguments().Any(g => g == typeof(TDevice))));
        }
    }
}