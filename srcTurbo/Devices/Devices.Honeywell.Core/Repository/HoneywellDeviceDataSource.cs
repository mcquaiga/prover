using Devices.Core.Interfaces;
using System;

namespace Devices.Honeywell.Core.Repository
{
    public static class HoneywellDeviceDataSourceFactory
    {
        public static IDeviceTypeDataSource<IDeviceType> Instance => _lazy.Value;

        public static IDeviceTypeDataSource<IDeviceType> GetInstance(string directory)
        {
            _directory = directory;
            return _lazy.Value;
        }

        private static readonly Lazy<IDeviceTypeDataSource<IDeviceType>> _lazy
                    = new Lazy<IDeviceTypeDataSource<IDeviceType>>(() => Factory());

        private static string _directory;

        private static IDeviceTypeDataSource<IDeviceType> Factory()
        {
            return new JsonDeviceTypeDataSource(new FileStreamReader(_directory));
        }
    }
}