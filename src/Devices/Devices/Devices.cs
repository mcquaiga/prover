using System.Threading.Tasks;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository.JsonRepository;
using Devices.Romet.Core.Repository;

namespace Devices
{
    public static class Devices
    {
        public static async Task<DeviceRepository> Repository()
        {
            await DeviceRepository.Instance.RegisterDataSourceAsync(MiJsonDeviceTypeDataSource.Instance);
            await DeviceRepository.Instance.RegisterDataSourceAsync(RometJsonDeviceTypeDataSource.Instance);

            return DeviceRepository.Instance;
        }
    }
}
