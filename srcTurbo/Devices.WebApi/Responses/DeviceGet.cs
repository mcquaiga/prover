using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Devices.WebApi.Responses
{
    public class DeviceGet
    {
        public string ConnectionRef => $"/api/devices/{Id}/Connect";

        [Key]
        [Required]
        public Guid Id { get; set; }

        public Link Link { get; set; }
        public string Type => _device.GetType().Name;
        public string Name => _device.Name;

        public DeviceGet(Guid id, IDevice device, IUrlHelper urlHelper)
        {
            Id = id;
            _device = device;

            _port = CommPorts.GetPorts().FirstOrDefault(x => x.Contains("COM"));
        }

        private readonly IDevice _device;
        private readonly string _port;
    }
}