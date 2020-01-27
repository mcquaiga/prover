using Devices.Core.Interfaces;
using System;

namespace Devices.Honeywell.Core.Repository
{
    public static class HoneywellDeviceDataSourceFactory
    {
        public static IDeviceDataSource<IDeviceType> Instance => _lazy.Value;

        public static IDeviceDataSource<IDeviceType> GetInstance(string directory)
        {
            _directory = directory;
            return _lazy.Value;
        }

        private static readonly Lazy<IDeviceDataSource<IDeviceType>> _lazy
                    = new Lazy<IDeviceDataSource<IDeviceType>>(() => Factory());

        private static string _directory;

        private static IDeviceDataSource<IDeviceType> Factory()
        {
            return new JsonDeviceDataSource(new FileStreamReader(_directory));
        }
    }
}