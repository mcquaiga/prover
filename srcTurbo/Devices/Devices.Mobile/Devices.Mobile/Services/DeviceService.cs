using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Devices.Mobile.Services
{
    public class DeviceService
    {
        public static string BaseUrl = $"http://10.0.2.2:5000";
        public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static DeviceService Instance { get { return _lazyDeviceService.Value; } }

        public ISubject<IDeviceInstance> DeviceInfoStream { get; private set; } = new Subject<IDeviceInstance>();

        public IConnectableObservable<string> ResponseMessageStream => _responseMessageStream.Publish();

        public DeviceService(HttpClient httpClient)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            _httpClient = httpClient;

            //Task.Run(async () => await FetchCommPortNames());
            //Task.Run(async () => await FetchDevices());
        }

        public async Task<ConnectionGetDto> ConnectToDeviceAsync(DeviceGetDto device, string portName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}{device.ConnectionRef}/{portName}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var conn = JsonConvert.DeserializeObject<ConnectionGetDto>(
                    json
                );

                //GetDeviceInfoTimed(conn);

                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> FetchCommPortNames()
        {
            try
            {
                if (_commPortCache?.Count == 0)
                {
                    Console.WriteLine("Fetching Comm ports from server.");
                    var response = await _httpClient.GetAsync($"{BaseUrl}/api/ports");

                    response.EnsureSuccessStatusCode();

                    _commPortCache = JsonConvert.DeserializeObject<HashSet<string>>(
                        await response.Content.ReadAsStringAsync()
                    );
                }

                return _commPortCache;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<DeviceGetDto>> FetchDevices()
        {
            try
            {
                if (_deviceCache?.Count == 0)
                {
                    Console.WriteLine("Fetching Devices from server.");
                    var response = await _httpClient.GetAsync($"{BaseUrl}/api/devices");

                    response.EnsureSuccessStatusCode();

                    _deviceCache = JsonConvert.DeserializeObject<HashSet<DeviceGetDto>>(
                        await response.Content.ReadAsStringAsync()
                    );
                }

                return _deviceCache;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IDeviceInstance> GetDeviceInfoAsync(ConnectionGetDto connection)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}{connection.DeviceRef}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IDeviceInstance>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        internal Task DisconnectAsync(ConnectionGetDto connection)
        {
            return _httpClient.GetAsync($"{BaseUrl}/api/connection/{connection.SessionId}/disconnect");
        }

        internal async Task<T> GetItemsAsync<T>(ConnectionGetDto connection) where T : IItemsGroup
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/connection/{connection.SessionId}/pressure");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private static Lazy<DeviceService> _lazyDeviceService = new Lazy<DeviceService>(()
            => new DeviceService(new HttpClient()));

        private HashSet<string> _commPortCache = new HashSet<string>();
        private HashSet<DeviceGetDto> _deviceCache = new HashSet<DeviceGetDto>();
        private HttpClient _httpClient;
        private ISubject<string> _responseMessageStream = new Subject<string>();

        private void GetDeviceInfoTimed(ConnectionGetDto conn)
        {
            Observable.Timer(TimeSpan.FromSeconds(3))
                .Subscribe(_ =>
                    Observable.StartAsync(() => GetDeviceInfoAsync(conn))
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(DeviceInfoStream)
            );
        }
    }
}