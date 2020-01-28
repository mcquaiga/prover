using Devices.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Devices.SignalRClient.ConsoleApp
{
    public class Link
    {
        public string Href { get; set; }
        public string Method { get; set; }
        public string Rel { get; set; }
    }

    internal class DeviceGet
    {
        public string ConnectionRef { get; set; }

        [Key]
        [Required]
        public Guid Id { get; set; }

        public Link Link { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}