using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces.Items;
using Devices.Core.Repository;
using Devices.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PressureItemController : DeviceControllerBase
    {
        public PressureItemController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager)
            : base(deviceRepository, sessionsManager)
        {
        }
    }
}