using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Communications.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortsController : ControllerBase
    {
        // GET: api/Ports
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return CommPorts.GetPorts();
        }
    }
}