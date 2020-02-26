using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Devices.Core.Repository
{
    public interface IDeviceRepository
    {
        ReadOnlyObservableCollection<DeviceType> Devices { get; }
        IObservableCache<DeviceType, Guid> All { get; }
        IObservable<IChangeSet<DeviceType, Guid>> Connect();
        void Dispose();
        IEnumerable<T> FilterCacheByType<T>() where T : DeviceType;
        T Find<T>(Func<T, bool> predicate) where T : DeviceType;
        IEnumerable<T> GetAll<T>() where T : DeviceType;
        IEnumerable<DeviceType> GetAll();
        DeviceType GetById(Guid id);
        DeviceType GetByName(string name);
        void Save();
        Task<DeviceRepository> UpdateCachedTypes();
        Task<DeviceRepository> UpdateCachedTypes(IDeviceTypeDataSource<DeviceType> dataSource);
        Task<DeviceRepository> UpdateCachedTypes(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources);
    }

    public class DeviceRepository : IDisposable, IDeviceRepository
    {
        public static IDeviceRepository Instance => _instance;

        private readonly SourceCache<DeviceType, Guid>
            _deviceSourceCache = new SourceCache<DeviceType, Guid>(v => v.Id);

        private readonly CompositeDisposable _disposer;
        private readonly ILogger _logger;

        internal IDeviceTypeCacheSource<DeviceType> CacheSource;
        private static IDeviceRepository _instance;

        public DeviceRepository(IDeviceTypeCacheSource<DeviceType> cacheRepository, ILogger logger = null) : this()
        {
            _logger = logger ?? NullLogger.Instance;

            CacheSource = cacheRepository;
        }

        public DeviceRepository() 
        {
            _logger = NullLogger.Instance;

            var devices = _deviceSourceCache.Connect().Publish();
            All = devices.AsObservableCache();

            var allDevice = All.Connect()
                .Bind(out var deviceTypes)
                .Subscribe();
            Devices = deviceTypes;

            _disposer = new CompositeDisposable(All, _deviceSourceCache, devices.Connect(), allDevice);

            _instance = this;
        }

        public ReadOnlyObservableCollection<DeviceType> Devices { get; }

        public IObservableCache<DeviceType, Guid> All { get; }

        public IObservable<IChangeSet<DeviceType, Guid>> Connect() => _deviceSourceCache.Connect();

        public void Dispose()
        {
            _disposer?.Dispose();
        }

        public IEnumerable<T> FilterCacheByType<T>() where T : DeviceType => All.Items.OfType<T>();

        public T Find<T>(Func<T, bool> predicate) where T : DeviceType
            => FilterCacheByType<T>().FirstOrDefault(predicate);

        public IEnumerable<T> GetAll<T>() where T : DeviceType => FilterCacheByType<T>();

        public IEnumerable<DeviceType> GetAll() => All.Items;

        public DeviceType GetById(Guid id) => All.Lookup(id).Value;

        public DeviceType GetByName(string name) => FindInSetByName(GetAll(), name);

        public void Save()
        {
            _logger.LogDebug("Saving device types to cache.");
            CacheSource.Save(_deviceSourceCache.Items);
        }

        //public async Task<bool> RegisterCacheSource(IDeviceTypeCacheSource<DeviceType> cacheSource)
        //{
        //    CacheSource = cacheSource;

        //    return await UpdateCachedTypes(cacheSource);
        //}

        public async Task<DeviceRepository> UpdateCachedTypes() => await UpdateCachedTypes(CacheSource);

        public async Task<DeviceRepository> UpdateCachedTypes(IDeviceTypeDataSource<DeviceType> dataSource)
        {
            if (dataSource == null)
                return this;

            _logger.LogDebug($"Loading devices for {dataSource.GetType()}.");

            await dataSource.GetDeviceTypes()
                .ForEachAsync(t => _deviceSourceCache.AddOrUpdate(t));

            return this;
        }

        public async Task<DeviceRepository> UpdateCachedTypes(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources)
        {
            foreach (var s in sources) await UpdateCachedTypes(s);
            //Save();
            return this;
        }

        private DeviceType FindInSetByName(IEnumerable<DeviceType> devices, string name)
        {
            var result = devices.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
            if (result == null)
                throw new KeyNotFoundException($"Device with name {name} not found.");

            return result;
        }
    }
}