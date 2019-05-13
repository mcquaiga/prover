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
        public ConnectController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager) : base(deviceRepository, sessionsManager)
        {
        }

        [Route("{id}/Connect")]
        [HttpGet("{id}/Connect/{port}", Name = nameof(GetDeviceConnection))]
        public async Task<ActionResult<ConnectionGet>> GetDeviceConnection(Guid id, string port)
        {
            //if (DeviceRepository.Devices.TryGetValue(id, out var device))
            //{
            //    var session = await SessionsManager.StartSession(device, port);

            //    return new ActionResult<ConnectionGet>(new ConnectionGet(session));
            //}
            return BadRequest("Device does not exist.");
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : DeviceControllerBase
    {
        public ConnectionController(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager) : base(deviceRepository, sessionsManager)
        {
        }

        // DELETE: api/ApiWithActions/5
        [Route("{id}/Disconnect")]
        [HttpDelete("{id}/Disconnect", Name = nameof(DeleteDisconnectAsync))]
        public async Task<ActionResult<string>> DeleteDisconnectAsync(Guid id)
        {
            var session = SessionsManager.Get(id);
            await session.Client.Disconnect();

            return new ActionResult<string>($"Disconnected from {session.Device.Name} at {DateTime.Now}");
        }

        [HttpGet(Name = nameof(GetConnections))]
        public ActionResult<IEnumerable<ConnectionGet>> GetConnections()
        {
            return new ActionResult<IEnumerable<ConnectionGet>>(SessionsManager.Get());
        }

        // GET: api/Sessions/5
        [HttpGet("{id}", Name = nameof(GetDeviceInfo))]
        public async Task<ActionResult<IDeviceWithValues>> GetDeviceInfo(Guid id)
        {
            var session = SessionsManager.Get(id);

            if (!session.Client.IsConnected)
                await session.Client.ConnectAsync();

            return new ActionResult<IDeviceWithValues>(session.Device);
        }

        [Route("{id}/Disconnect")]
        [HttpGet("{id}/Disconnect", Name = nameof(GetDisconnectAsync))]
        public async Task<ActionResult<string>> GetDisconnectAsync(Guid id)
        {
            var session = await SessionsManager.EndSession(id);

            return new ActionResult<string>($"Disconnected from {session.Device.Name} at {DateTime.Now}");
        }

        // GET: api/Sessions/5
        [Route("{id}/Pressure")]
        [HttpGet("{id}/Pressure", Name = nameof(GetPressureItems))]
        public async Task<ActionResult<IPressureItems>> GetPressureItems(Guid id)
        {
            var session = SessionsManager.Get(id);
            var items = await session.Client.GetItemsAsync<IPressureItems>();
            return new ActionResult<IPressureItems>(items);
        }
    }
}