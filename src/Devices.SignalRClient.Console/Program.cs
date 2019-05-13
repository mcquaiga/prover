using Devices.Core.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Devices.SignalRClient.ConsoleApp
{
    public class DeviceConnectionManager
    {
        public DeviceConnectionManager()
        {
            _httpClient = new HttpClient();
            _hubConnection = new HubConnectionBuilder()
               .WithUrl(HubUrl)
               .Build();

            _hubConnection.On<Guid>("Connected",
                d =>
                {
                    Console.WriteLine($"Connected: {d.ToString()}");
                });

            _hubConnection.On("Disconnected", () => Console.WriteLine("Disconnected!"));

            _hubConnection.On<string>("ReceiveDeviceInfo",
                d => Console.WriteLine($"Device: {d}"));

            _hubConnection.On<string>("ReceiveStatusUpdate", Console.WriteLine);
        }

        public Task GetPressureItems()
        {
            return _hubConnection.SendAsync("GetPressureItems");
        }

        public Task GetSiteInfo()
        {
            return _hubConnection.SendAsync("GetDeviceInfo");
        }

        public async Task Start()
        {
            var response = await _httpClient.GetAsync(DeviceApiUrl);
            response.EnsureSuccessStatusCode();
            var devicesJson = await response.Content.ReadAsStringAsync();
            var devices = JsonConvert.DeserializeObject<IEnumerable<DeviceGet>>(devicesJson);

            var deviceId = devices.FirstOrDefault(x => x.Name == "Mini-AT")?.Id;

            await _hubConnection.StartAsync();

            await _hubConnection.SendCoreAsync("StartConnection", new object[] { deviceId, CommPort });
        }

        public Task Stop()
        {
            return _hubConnection.SendAsync("StopConnection");
        }

        private static string CommPort = "COM4";

        private static string DeviceApiUrl = "http://localhost:5000/api/devices";

        private static string HubUrl = "http://localhost:5000/deviceHub";

        private readonly HttpClient _httpClient;

        //public static IHubProxy DeviceHubProxy { get; private set; }
        private readonly HubConnection _hubConnection;
    }

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var manager = new DeviceConnectionManager();
            var exit = false;

            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("Enter command (help to display help): ");
                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "start":
                        Task.Run(async () => await manager.Start());
                        break;

                    case "stop":
                        await manager.Stop();
                        break;

                    case "info":
                        await manager.GetSiteInfo();
                        break;

                    case "press":
                        await manager.GetPressureItems();
                        break;

                    case "exit":
                        exit = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}