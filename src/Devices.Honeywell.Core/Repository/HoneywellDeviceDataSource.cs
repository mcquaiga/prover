using Devices.Core.Interfaces;
using System;

namespace Devices.Honeywell.Core.Repository
{
    public static class DeviceDataSourceFactory
    {
        #region Methods

        public static IDeviceDataSource<IHoneywellDeviceType> Instance => _lazy.Value;

        private static readonly Lazy<IDeviceDataSource<IHoneywellDeviceType>> _lazy
            = new Lazy<IDeviceDataSource<IHoneywellDeviceType>>(() => Factory());

        private static IDeviceDataSource<IHoneywellDeviceType> Factory()
        {
            return new JsonDeviceDataSource(new FileStreamReader());
        }

        #endregion
    }
}