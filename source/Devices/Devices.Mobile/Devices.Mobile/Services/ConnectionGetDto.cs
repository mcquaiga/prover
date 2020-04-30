using Newtonsoft.Json;
using System;

namespace Devices.Mobile.Services
{
    public class ConnectionGetDto
    {
        public string DeviceRef { get; set; }

        public bool IsConnected { get; set; } = false;

        public string SerialNumber { get; set; }

        public Guid SessionId { get; set; }

        public string Status { get; set; }

        [JsonConstructor]
        public ConnectionGetDto(Guid sessionId)
        {
            SessionId = sessionId;

            Status = "Your request has been queued to run. Check the DeviceRef url shortly.";
        }

        public ConnectionGetDto()
        {
        }
    }
}