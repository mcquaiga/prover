using System;
using System.Collections.Generic;
using System.Text;
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