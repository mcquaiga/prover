using System;
using System.Linq;
using System.Reflection;
using Devices.Communications.IO;
using Devices.Core.Interfaces;

namespace Devices.Communications.Interfaces
{
    public class CommunicationsClientFactory : ICommClientFactory
    {
        private static readonly Assembly[] _assemblies =
        {
            Assembly.Load("Devices.Honeywell.Comm"),
            Assembly.Load("Devices.Romet.Comm")
        };

        private readonly ICommPortFactory _commFactory;

        public CommunicationsClientFactory(ICommPortFactory commFactory) => _commFactory = commFactory;

        #region ICommClientFactory Members

        public ICommunicationsClient Create<T>(T deviceType, string portName, int? baudRate = null) where T : DeviceType
        {
            var commPort = _commFactory.Create(portName, baudRate);
            return Create(deviceType, commPort);
        }

        public ICommunicationsClient Create<T>(T deviceType, ICommPort commPort) where T : DeviceType
        {
            var factory = LocateFactory(deviceType);
            var clientFactory = Activator.CreateInstance(factory);
            var method = factory.GetMethod("Create");

            return (ICommunicationsClient) method?.Invoke(clientFactory, new object[] {deviceType, commPort});
        }

        #endregion

        private static TypeInfo LocateFactory<T>(T deviceType)
        {
            var factory = _assemblies.Select(a => a.DefinedTypes.FirstOrDefault(t =>
                t.ImplementedInterfaces.Any(i => i.Name == typeof(IDeviceTypeCommClientFactory<>).Name
                                                 && i.GenericTypeArguments.Contains(deviceType.GetType()))
                && !t.IsInterface && !t.IsAbstract)).LastOrDefault();


            if (factory == null)
                throw new ArgumentNullException($"Could not locate factory method for device type {typeof(T)}.");


            return factory;
        }
    }
}