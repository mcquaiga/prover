using System;
using System.Threading.Tasks;

namespace Prover.Shared.Hubs
{
    public interface IDeviceHub
    {
        Task Connect(Guid sessionId);

        Task Disconnect();

        Task GetDeviceItemValues();
    }
}