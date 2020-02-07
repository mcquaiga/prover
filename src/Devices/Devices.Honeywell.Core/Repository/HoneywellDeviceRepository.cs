using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
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
            return DeviceRepository.Instance.RegisterDataSource(DataSource);
        }
    }
}