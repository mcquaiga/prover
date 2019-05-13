using Core.Hubs;
using Devices.Core.Interfaces.Items;
using Devices.Core.Repository;
using Devices.WebApi.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Devices.WebApi.Hubs
{
    public class DeviceHub : Hub
    {
        public DeviceRepository DeviceRepository { get; }

        public RemoteConnectionsManager SessionsManager { get; }

        public DeviceHub(DeviceRepository deviceRepository, RemoteConnectionsManager sessionsManager)
        {
            DeviceRepository = deviceRepository;
            SessionsManager = sessionsManager;
        }

        public Task GetDeviceInfo()
        {
            var sessId = SessionsManager.Get().FirstOrDefault().SessionId;
            var conn = SessionsManager.Get(sessId);
            return Clients.Caller.SendAsync("ReceiveDeviceInfo", JsonConvert.SerializeObject(conn.Device.SiteInfo, Formatting.Indented));
        }

        public async Task GetPressureItems()
        {
            var sessId = SessionsManager.Get().FirstOrDefault().SessionId;
            var conn = SessionsManager.Get(sessId);

            var pressure = await conn.Client.GetItemsAsync<IPressureItems>();
            await Clients.Caller.SendAsync("ReceiveDeviceInfo", JsonConvert.SerializeObject(pressure, Formatting.Indented));
        }

        public override Task OnConnectedAsync()
        {
            //var success = Context.Items.TryGetValue("deviceId", out var deviceId);
            //success = Context.Items.TryGetValue("portName", out var portName);

            //if (success)
            //{
            //    DeviceRepository.Devices.TryGetValue((Guid)deviceId, out var device);

            //    return SessionsManager.StartSession(device, portName.ToString());
            //}

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task StartConnection(Guid deviceId, string portName)
        {
            if (DeviceRepository.Devices.TryGetValue(deviceId, out var device))
            {
                SessionsManager.ConnectionStatusObservable
                    .Subscribe(rd => Clients.Caller.SendAsync("Connected", rd.Id));

                var status = new Subject<string>();
                status.Subscribe(x => Clients.Caller.SendAsync("ReceiveStatusUpdate", x));

                var sessionId = await SessionsManager.StartSession(device, portName,
                    status);
            }
        }

        public async Task StopConnection()
        {
            var sessId = SessionsManager.Get().FirstOrDefault().SessionId;
            await SessionsManager.EndSession(sessId);
            await Clients.Caller.SendAsync("Disconnected");
        }
    }
}