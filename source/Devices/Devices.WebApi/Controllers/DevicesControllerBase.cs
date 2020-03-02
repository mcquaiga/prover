using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Repository;
using Devices.WebApi.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.WebApi.Controllers
{
    [Controller]
    public abstract class DeviceControllerBase : ControllerBase
    {
        public DeviceControllerBase(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager)
        {
            DeviceRepository = deviceRepository;
            SessionsManager = sessionsManager;
        }

        protected readonly DeviceRepository DeviceRepository;
        protected readonly RemoteConnectionsManager SessionsManager;
    }
}