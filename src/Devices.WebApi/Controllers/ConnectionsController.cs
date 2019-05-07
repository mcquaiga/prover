using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Communications;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Core.Repository;
using Devices.WebApi.Responses;
using Devices.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.WebApi.Controllers
{
    [Route("api/Devices/")]
    [ApiController]
    public class ConnectController : DeviceControllerBase
    {
        public ConnectController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager, ICommPort commPort) : base(deviceRepository, sessionsManager)
        {
            _commPort = commPort;
        }

        [Route("{id}/Connect")]
        [HttpGet("{id}/Connect/{port}", Name = nameof(GetDeviceConnection))]
        public async Task<ActionResult<ConnectionGet>> GetDeviceConnection(Guid id, string port)
        {
            if (DeviceRepository.Devices.TryGetValue(id, out var device))
            {
                _commPort.Setup(port, 9600);
                var session = await SessionsManager.StartSession(device, _commPort);

                return new ActionResult<ConnectionGet>(new ConnectionGet(session));
            }
            return BadRequest();
        }

        private readonly ICommPort _commPort;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : DeviceControllerBase
    {
        public ConnectionController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager, ICommPort commPort) : base(deviceRepository, sessionsManager)
        {
            _commPort = commPort;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }

        [Route("{id}/Disconnect")]
        [HttpGet("{id}/Disconnect", Name = nameof(Disconnect))]
        public async Task<ActionResult<string>> Disconnect(Guid id)
        {
            var mydevice = SessionsManager.Get(id);
            await mydevice.Item1.Disconnect();

            return new ActionResult<string>($"Disconnected from {mydevice.Item2.Name} at {DateTime.Now}");
        }

        // GET: api/Sessions/5
        [HttpGet("{id}", Name = nameof(GetDeviceInfo))]
        public async Task<ActionResult<IDeviceWithValues>> GetDeviceInfo(Guid id)
        {
            var mydevice = SessionsManager.Get(id);

            if (!mydevice.Item1.IsConnected)
                await mydevice.Item1.ConnectAsync();

            return new ActionResult<IDeviceWithValues>(mydevice.Item2);
        }

        // GET: api/Sessions/5
        [Route("{id}/Pressure")]
        [HttpGet("{id}/Pressure", Name = nameof(GetPressureItems))]
        public async Task<ActionResult<IPressureItems>> GetPressureItems(Guid id)
        {
            var mydevice = SessionsManager.Get(id);
            return new ActionResult<IPressureItems>(mydevice.Item2.Pressure);
        }

        private readonly ICommPort _commPort;
    }
}