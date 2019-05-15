using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core.Repository;
using ReactiveUI;
using Splat;
using System.Reflection;

namespace Devices.Terminal.Wpf
{
    public static class Bootstrapper
    {
        public static void Start()
        {
            Locator.CurrentMutable.Register(() => HoneywellDeviceDataSourceFactory.Instance, typeof(IDeviceDataSource<IDevice>));

            Locator.CurrentMutable.Register(() => new DeviceRepository(Locator.Current.GetServices<IDeviceDataSource<IDevice>>()), typeof(DeviceRepository));

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
        }
    }
}