using Devices.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Devices.WebApi.Responses
{
    public class ConnectionGet
    {
        public string DeviceRef => $"/api/connection/{SessionId}";
        public Link Link { get; set; }

        [Key]
        public Guid SessionId { get; set; }

        public string Status { get; protected set; }
        public string SerialNumber => _device?.SiteInfo.SerialNumber;

        public ConnectionGet(Guid sessionId, IDeviceWithValues device)
        {
            SessionId = sessionId;
            _device = device;
        }

        public ConnectionGet(Guid sessionId)
        {
            SessionId = sessionId;

            Status = "Your request has been queued to run. Check the DeviceRef url shortly.";
        }

        private readonly IDeviceWithValues _device;
    }

    public class DisconnectGet : ConnectionGet
    {
        public DisconnectGet(Guid sessionId, IDeviceWithValues device) : base(sessionId, device)
        {
            Status = $"Disconnected from {device.Name}. Reconnect with the DeviceRef";
        }
    }
}