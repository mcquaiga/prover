using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;
using DynamicData;

namespace Devices
{
    public static class Repository
    {
        //private static readonly Lazy<DeviceRepository> Lazy = new Lazy<DeviceRepository>(RegisterDataSources);
        private static readonly Lazy<Task<DeviceRepository>> LazyAsync = new Lazy<Task<DeviceRepository>>(RegisterDataSourcesAsync);
        //private static readonly Lazy<DeviceRepository> Lazy = new Lazy<DeviceRepository>(RegisterDataSources);

        //public static DeviceRepository Instance => Lazy.Value;
        public static IObservable<IChangeSet<DeviceType>> Connect(this DeviceRepository repo)
        {
            return repo.Connect();
        }
        private static DeviceRepository RegisterDataSources()
        {
            var repo = new DeviceRepository();

            repo.RegisterDataSource(MiJsonDeviceTypeDataSource.Instance);
            repo.RegisterDataSource(RometJsonDeviceTypeDataSource.Instance);

            return repo;
        }

        private static async Task<DeviceRepository> RegisterDataSourcesAsync()
        {
            var repo = new DeviceRepository();

            await repo.RegisterDataSourceAsync(MiJsonDeviceTypeDataSource.Instance);
            await repo.RegisterDataSourceAsync(RometJsonDeviceTypeDataSource.Instance);

            return repo;
        }

        public static async Task<DeviceRepository> GetAsync()
        {
            return await LazyAsync.Value;
        }

        public static DeviceRepository Get()
        {
            var t= Task.Run(GetAsync);
            t.Wait();
            return t.Result;
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this Lazy<Task<T>> asyncTask){
            return asyncTask.Value.GetAwaiter();
        }
    }
}