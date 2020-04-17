using Devices.Core.Interfaces;
using Prover.Shared.Interfaces;

namespace Devices.Communications.Interfaces
{
    public interface IDeviceTypeCommClientFactory<in TDevice>
        where TDevice : DeviceType
    {
    ICommunicationsClient Create(TDevice deviceType, ICommPort commPort);
    }

    public interface ICommClientFactory
    { 
        ICommunicationsClient Create<T>(T deviceType, string portName = null, int? baudRate = null) where T : DeviceType;
        ICommunicationsClient Create<T>(T deviceType, ICommPort commPort) where T : DeviceType;
    }
}