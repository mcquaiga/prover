using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Communications.Messaging;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Items;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.Communications
{
    public static class DeviceConnection
    {
        public static Task<ICommunicationsClient> ConnectAsync<T>(this T deviceType, ICommPort commPort, int retryAttempts = 10, TimeSpan? timeout = null)
            where T : IDevice
        {
            var a = Assembly.Load("Devices.Honeywell.Comm");
            var factory = a.GetExportedTypes().FirstOrDefault(t => t.Name.Contains("Factory"));
            var obj = Activator.CreateInstance(factory);
            var method = factory.GetMethod("Create");

            return (Task<ICommunicationsClient>)method.Invoke(obj, new object[] { deviceType, commPort, retryAttempts, timeout });
        }
    }
}