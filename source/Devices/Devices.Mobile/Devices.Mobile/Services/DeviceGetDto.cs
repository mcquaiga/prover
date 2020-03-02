using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace Devices.Mobile.Services
{
    public class DeviceGetDto
    {
        public string ConnectionRef { get; set; }

        public Guid Id { get; set; }

        public LinkDto Link { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class DisconnectGetDto : ConnectionGetDto
    {
        public DisconnectGetDto()
        {
        }

        public DisconnectGetDto(Guid sessionId) : base(sessionId)
        {
            Status = $"Disconnected from. Reconnect with the DeviceRef";
        }
    }

    public class LinkDto
    {
        public string Href { get; set; }
        public string Method { get; set; }
        public string Rel { get; set; }

        public LinkDto(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }

        public LinkDto()
        {
        }
    }
}