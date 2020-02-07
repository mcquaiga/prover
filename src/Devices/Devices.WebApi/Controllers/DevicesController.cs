using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Communications;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Devices.WebApi.Responses;
using Devices.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Devices.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : DeviceControllerBase
    {
        public DevicesController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager, IUrlHelper urlHelper)
            : base(deviceRepository, sessionsManager)
        {
            _urlHelper = urlHelper;
        }

        // GET api/values
        [HttpGet(Name = nameof(GetDevices))]
        public async Task<IActionResult> GetDevices()
        {
            return new ObjectResult(
                    DeviceRepository.Devices
                        .Select(d => new DeviceGet(d.Key, d.Value, _urlHelper))
                        .OrderBy(x => x.Name)
                );
        }

        private readonly IUrlHelper _urlHelper;
    }
}