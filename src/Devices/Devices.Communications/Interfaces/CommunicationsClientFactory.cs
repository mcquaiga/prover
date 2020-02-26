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

        private CommunicationsClientFactory(){}

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

        public static ICommunicationsClient CreateClient<T>(T deviceType, ICommPort commPort) where T : DeviceType
        {
            return new CommunicationsClientFactory().Create(deviceType, commPort);
        }
        #endregion

        private static TypeInfo LocateFactory<T>(T deviceType)
        {
            foreach (var assembly in _assemblies)
            {
                var factory = assembly.DefinedTypes.FirstOrDefault(t => 
                    !t.IsInterface && !t.IsAbstract && t.ImplementedInterfaces.Any(i => 
                        i.Name == typeof(IDeviceTypeCommClientFactory<>).Name && i.GenericTypeArguments.Contains(deviceType.GetType())) 
                    );

                if (factory != null)
                    return factory;
            }
            
            throw new ArgumentNullException($"Could not locate factory method for device type {typeof(T)}.");
        }
    }
}