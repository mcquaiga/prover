using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devices.Core.Interfaces
{
    public interface ILoadDeviceTypes
    {
        Task<IEnumerable<IEvcDeviceType>> LoadDevicesAsync();
    }
}
