using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Devices.Mobile.Services
{
    public class DeviceSignalRClient
    {
        public DeviceSignalRClient(string url, Guid deviceId, string portName)
        {
            var queryString = new Dictionary<string, string>();
            queryString.Add("deviceId", deviceId.ToString());
            queryString.Add("portName", portName);

            Connection = new HubConnection(url, queryString);

            DeviceHubProxy = Connection.CreateHubProxy("DeviceHub");
        }

        public async Task Connect()
        {
            await Connection.Start();
        }

        private HubConnection Connection;
        private IHubProxy DeviceHubProxy;
    }
}