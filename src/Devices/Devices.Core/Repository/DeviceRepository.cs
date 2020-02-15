using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Shared.Queues;

namespace Devices.Core.Repository
{
    public class DeviceRepository
    {
        private readonly ConcurrentDictionary<DeviceType, bool> _deviceCache = new ConcurrentDictionary<DeviceType, bool>();

        private readonly ConcurrentDictionary<IDeviceTypeDataSource<DeviceType>, bool> _deviceDataSources
            = new ConcurrentDictionary<IDeviceTypeDataSource<DeviceType>, bool>();

        private readonly BackgroundQueue _queue = new BackgroundQueue();

        private readonly DateTime _timeCreated = DateTime.UtcNow;

        private readonly SourceList<DeviceType> _deviceSourceCache = new SourceList<DeviceType>();

        // We expose the Connect() since we are interested in a stream of changes.
        // If we have more than one subscriber, and the subscribers are known, 
        // it is recommended you look into the Reactive Extension method Publish().
        public IObservable<IChangeSet<DeviceType>> Connect() => _deviceSourceCache.Connect();

        public DeviceRepository()
        {
        }

        #region Public Properties

        #endregion

        #region Public Methods

        public IEnumerable<T> FilterCacheByType<T>() where T : DeviceType
        {
            var results = _deviceCache.OfType<T>();
            return results;
        }

        public IEnumerable<IDeviceTypeDataSource<DeviceType>> FilterDataSourceTypes<TDevice>()
            where TDevice : DeviceType
        {
            return _deviceDataSources
                .Select(t => t.Key)
                .Where(s =>
                    s.GetType().GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDeviceTypeDataSource<>))
                        .Any(t => t.GetGenericArguments().Any(g => g == typeof(TDevice))));
        }

        public T Find<T>(Func<T, bool> predicate) where T : DeviceType
        {
            return FilterCacheByType<T>().FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetAll<T>() where T : DeviceType
        {
            return FilterCacheByType<T>();
        }

        public IEnumerable<DeviceType> GetAll()
        {
            return _deviceCache
                .Select(kv => kv.Key)
                .ToList();
        }

        public DeviceType GetByName(string name)
        {
            return FindInSetByName(GetAll(), name);
        }

        public DeviceType GetById(Guid id)
        {
            return _deviceCache.FirstOrDefault(d => d.Key.Id == id).Key;
        }

        public T GetByName<T>(string name)
            where T : DeviceType
        {
            var r = FilterCacheByType<T>();
            return (T) FindInSetByName(r, name);
        }

        public DeviceRepository RegisterDataSource(IDeviceTypeDataSource<DeviceType> dataSource)
        {
            return RegisterDataSource(new[] {dataSource});
        }

        public DeviceRepository RegisterDataSource(IEnumerable<IDeviceTypeDataSource<DeviceType>> dataSources)
        {
            var task = Task.Run(() => RegisterDataSourceAsync(dataSources));
            task.Wait();
            return task.Result;
        }

        public async Task<DeviceRepository> RegisterDataSourceAsync(IDeviceTypeDataSource<DeviceType> dataSource)
        {
            return await RegisterDataSourceAsync(new[] {dataSource});
        }

        public async Task<DeviceRepository> RegisterDataSourceAsync(IEnumerable<IDeviceTypeDataSource<DeviceType>> dataSources)
        {
            var uniques = dataSources.Except(_deviceDataSources.Select(k => k.Key)).ToList();
            uniques
                .ForEach(u => _deviceDataSources.GetOrAdd(u, false));

            await LoadFromDataSources(uniques);
            
            return this;
        }

        #endregion

        #region Private

        private DeviceType FindInSetByName(IEnumerable<DeviceType> devices, string name)
        {
            var result = devices.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
            if (result == null)
                throw new KeyNotFoundException($"Device with name {name} not found.");

            return result;
        }

        private async Task LoadFromDataSources(IEnumerable<IDeviceTypeDataSource<DeviceType>> dataSources)
        {
            foreach (var ds in dataSources)
            {
                await ds.GetDeviceTypes()
                    .ForEachAsync(d =>
                    {
                        _deviceCache.GetOrAdd(d, true);
                        _deviceSourceCache.Add(d);
                    });
                
                _deviceDataSources.TryUpdate(ds, true, false);
            }
        }

        #endregion
    }
}