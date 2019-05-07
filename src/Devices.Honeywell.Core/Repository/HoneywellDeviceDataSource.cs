using Devices.Core.Interfaces;
using System;

namespace Devices.Honeywell.Core.Repository
{
    public static class HoneywellDeviceDataSourceFactory
    {
        public static IDeviceDataSource<IDevice> Instance => _lazy.Value;

        public static IDeviceDataSource<IDevice> GetInstance(string directory)
        {
            _directory = directory;
            return _lazy.Value;
        }

        private static readonly Lazy<IDeviceDataSource<IDevice>> _lazy
                    = new Lazy<IDeviceDataSource<IDevice>>(() => Factory());

        private static string _directory;

        private static IDeviceDataSource<IDevice> Factory()
        {
            return new JsonDeviceDataSource(new FileStreamReader(_directory));
        }
    }
}