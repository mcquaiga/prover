using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devices.Communications
{
    public static class DeviceConnection
    {
        public static Task<ICommunicationsClient> ConnectAsync<T>(this T deviceType, ICommPort commPort, int retryAttempts = 10, TimeSpan? timeout = null, IObserver<string> statusObserver = null)
            where T : IDeviceType
        {
            var type = typeof(T);
            var a = Assembly.Load(type.Assembly.ToString());
            var factory = a.GetExportedTypes().FirstOrDefault(t => t.Name.Contains("Factory"));
            var obj = Activator.CreateInstance(factory);
            var method = factory.GetMethod("Create");

            return (Task<ICommunicationsClient>)method.Invoke(obj, new object[] { deviceType, commPort, retryAttempts, timeout, statusObserver });
        }
    }
}