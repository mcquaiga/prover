using Autofac;
using Devices.Communications.IO;
using Devices.Communications.IrDa;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository;
using ReactiveUI.Autofac;
using Splat;

namespace Devices.Terminal.Wpf
{
    public static class Bootstrapper
    {
        #region Properties

        public static IContainer Container { get; private set; }

        #endregion

        #region Methods

        public static void Start()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => DeviceDataSourceFactory.Instance)
                .As<IDeviceDataSource<IHoneywellDeviceType>>()
                .As<IDeviceDataSource<IDeviceType>>();
            builder.RegisterType<DeviceRepository>()
                .SingleInstance();

            builder.RegisterType<SerialPort>().As<ICommPort>();
            builder.RegisterType<IrDAPort>().As<ICommPort>();

            builder.RegisterForReactiveUI();

            RxAppAutofacExtension.UseAutofacDependencyResolver(builder.Build());

            Container = builder.Build();
        }

        #endregion
    }
}