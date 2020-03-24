using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using Microsoft.Extensions.Logging;
using Prover.Infrastructure;

namespace Devices
{
    public class RepositoryFactory
    {
        public RepositoryFactory(ILogger logger)
        {
        }

        //if (DeviceRepository.Instance == null)
        //    throw new InvalidOperationException("Call Create method first.");
        public static IDeviceRepository Instance => DeviceRepository.Instance;

        public static IDeviceRepository Get => Instance;

        public static async Task<IDeviceRepository> Create(IDeviceTypeCacheSource<DeviceType> cacheSource,
            IEnumerable<IDeviceTypeDataSource<DeviceType>> sources = null)
        {
            var instance = Instance;

            if (Instance == null)
            {
                instance = new DeviceRepository(cacheSource);
            }

            await instance.UpdateCachedTypes();

            if (instance.Devices.Count == 0 && sources != null)
                await instance.UpdateCachedTypes(sources);

            return instance;
        }

        public static async Task<IDeviceRepository> Create(IEnumerable<IDeviceTypeDataSource<DeviceType>> sources)
        {
            var instance = Instance;

            if (Instance == null)
                instance = new DeviceRepository();

            await instance.UpdateCachedTypes(sources);

            return instance;
        }

        public static IDeviceRepository CreateDefault()
        {
            var task = Task.Run(CreateDefaultAsync);
            task.Wait();
            return task.Result;
        }

        public static async Task<IDeviceRepository> CreateDefaultAsync()
        {
            return await Create(StorageDefaults.CreateDefaultDeviceTypeCache(),
                new[] {MiJsonDeviceTypeDataSource.Instance, RometJsonDeviceTypeDataSource.Instance});
        }
    }
}