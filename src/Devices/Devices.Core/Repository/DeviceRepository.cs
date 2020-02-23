using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;

namespace Devices.Core.Repository
{
    public class DeviceRepository : IDisposable
    {
        private readonly SourceList<IDeviceTypeDataSource<DeviceType>> _dataSources =
            new SourceList<IDeviceTypeDataSource<DeviceType>>();

        private readonly SourceCache<DeviceType, Guid>
            _deviceSourceCache = new SourceCache<DeviceType, Guid>(v => v.Id);

        private readonly CompositeDisposable _disposer;

        internal IDeviceTypeCacheSource<DeviceType> CacheSource;

        public DeviceRepository(IDeviceTypeCacheSource<DeviceType> cacheRepository) : this() =>
            CacheSource = cacheRepository;

        public DeviceRepository()
        {
            var devices = _deviceSourceCache.Connect().Publish();
            All = devices.AsObservableCache();

            var allDevice = All.Connect()
                .Bind(out var deviceTypes)
                .Subscribe();
            Devices = deviceTypes;

            _disposer = new CompositeDisposable(All, _deviceSourceCache, _dataSources, devices.Connect(), allDevice);
        }

        public ReadOnlyObservableCollection<DeviceType> Devices { get; }

        public IObservableCache<DeviceType, Guid> All { get; }

        #region IDisposable Members

        public void Dispose()
        {
            _deviceSourceCache?.Dispose();
            _dataSources?.Dispose();
            _disposer?.Dispose();
            All?.Dispose();
        }

        #endregion

        public IObservable<IChangeSet<DeviceType, Guid>> Connect() => _deviceSourceCache.Connect();

        public IEnumerable<T> FilterCacheByType<T>() where T : DeviceType => All.Items.OfType<T>();

        public T Find<T>(Func<T, bool> predicate) where T : DeviceType
            => FilterCacheByType<T>().FirstOrDefault(predicate);

        public IEnumerable<T> GetAll<T>() where T : DeviceType => FilterCacheByType<T>();

        public IEnumerable<DeviceType> GetAll() => All.Items;

        public DeviceType GetById(Guid id) => All.Lookup(id).Value;

        public DeviceType GetByName(string name) => FindInSetByName(GetAll(), name);

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

            await dataSource.GetDeviceTypes()
                .ForEachAsync(t => _deviceSourceCache.AddOrUpdate(t));

            return this;
        }

        public void Save()
        {
            CacheSource.Save(_deviceSourceCache.Items);
        }

        public async Task<DeviceRepository> UpdateCachedTypes(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources)
        {
            foreach (var s in sources) await UpdateCachedTypes(s);

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