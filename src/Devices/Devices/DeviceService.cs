using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;

namespace Devices
{
    public class RepositoryFactory
    {
        private static DeviceRepository _instance;

        public static DeviceRepository Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Call Create method first.");

                return _instance;
            }
        }

        public static DeviceRepository Get => Instance;

        public static async Task<DeviceRepository> Create(IDeviceTypeCacheSource<DeviceType> cacheSource,
            IEnumerable<IDeviceTypeDataSource<DeviceType>> sources = null)
        {
            if (_instance == null)
                _instance = new DeviceRepository(cacheSource);

            await _instance.UpdateCachedTypes();

            if (_instance.Devices.Count == 0 && sources != null)
                await _instance.UpdateCachedTypes(sources);

            return _instance;
        }

        public static async Task<DeviceRepository> Create(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources)
        {
            if (_instance == null)
               _instance = new DeviceRepository();

            await _instance.UpdateCachedTypes(sources);
            return _instance;
        }

        public static async Task<DeviceRepository> CreateDefaultAsync()
        {
            return await Create(new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance});
        }

        public static DeviceRepository CreateDefault()
        {
            var task = Task.Run(() => Create(new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance}));
            task.Wait();
            return task.Result;
        }
    }
}