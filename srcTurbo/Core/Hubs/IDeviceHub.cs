using System;
using System.Threading.Tasks;

namespace Core.Hubs
{
    public interface IDeviceHub
    {
        Task Connect(Guid sessionId);

        Task Disconnect();

        Task GetDeviceItemValues();
    }
}