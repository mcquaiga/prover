using System;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;

namespace Devices.Honeywell.Core.Repository
{
    public static class HoneywellDeviceRepository
    {
        public static IDeviceTypeDataSource<DeviceType> DataSource => MiJsonDeviceTypeDataSource.Instance;

        public static DeviceRepository Devices => _lazy.Value;
        private static readonly Lazy<DeviceRepository> _lazy = new Lazy<DeviceRepository>(Factory);
        
        private static DeviceRepository Factory()
        {
            return new DeviceRepository().RegisterDataSource(DataSource);
        }
    }
}