﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private DeviceRepository _deviceRepository;

        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ILogger<DevicesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task< IEnumerable<DeviceType>> GetDeviceTypes()
        {
            _deviceRepository = await Devices.RepositoryFactory.CreateDefaultAsync();

            return _deviceRepository.GetAll();
        }
    }
}
