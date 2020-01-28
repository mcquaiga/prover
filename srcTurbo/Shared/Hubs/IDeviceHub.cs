using System;
using System.Threading.Tasks;

namespace Shared.Hubs
{
    public interface IDeviceHub
    {
        Task Connect(Guid sessionId);

        Task Disconnect();

        Task GetDeviceItemValues();
    }
}