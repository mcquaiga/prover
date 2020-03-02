using System.Collections.Generic;
using Devices.Core.Interfaces;

namespace Devices.Core.Repository
{
    public interface IDeviceTypeCacheSource<T> : IDeviceTypeDataSource<T>
        where T : DeviceType
    {
        void Save(IEnumerable<T> values);
    }
}