using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using Devices.Mobile.Services;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace Devices.Mobile.ViewModels
{
    public class NewConnectionViewModel : BaseViewModel
    {
        public static string BaseUrl = $"http://10.0.2.2:5000/deviceHub";

        public ConnectionGetDto ConnectionInfo { [ObservableAsProperty] get; }

        //public ReactiveCommand<Unit, ConnectionGetDto> CreateConnectionCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateConnectionCommand { get; }

        [Reactive]
        public DeviceGetDto Device { get; set; }

        public DeviceSignalRClient DeviceClient { get; }

        [Reactive]
        public IDeviceInstance DeviceInstance { get; set; }

        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        public bool IsConnected { [ObservableAsProperty] get; }

        [Reactive]
        public string PortName { get; set; }

        public ReactiveCommand<Unit, IPressureItems> PressureItemsCommand { get; }

        //public string ResponseMessage { [ObservableAsProperty] get; }
        [Reactive]
        public string ResponseMessage { get; set; }

        public NewConnectionViewModel(DeviceGetDto device, string portName)
        {
            Device = device;
            PortName = portName;

            DeviceClient = new DeviceSignalRClient(BaseUrl, device.Id, portName);

            //CreateConnectionCommand = ReactiveCommand.CreateFromTask(
            //    async () => await DeviceService.ConnectToDeviceAsync(Device, PortName)
            //);

            CreateConnectionCommand = ReactiveCommand.CreateFromTask(
                async () => await DeviceClient.Connect()
            );

            DisconnectCommand = ReactiveCommand.CreateFromTask(
                    async () => await DeviceService.DisconnectAsync(ConnectionInfo)
            );

            PressureItemsCommand = ReactiveCommand.CreateFromTask(
                    async () => await DeviceService.GetItemsAsync<IPressureItems>(ConnectionInfo),
                    this.WhenAny(x => x.ConnectionInfo, x => x != null)
            );

            ////CreateConnectionCommand
            ////    .ToPropertyEx(this, x => x.ConnectionInfo);

            //this.WhenAnyValue(x => x.ConnectionInfo)
            //    .Select(ci => JsonConvert.SerializeObject(ci, Formatting.Indented))
            //    .ToPropertyEx(this, x => x.ResponseMessage);

            //this.WhenAnyValue(x => x.ConnectionInfo.IsConnected)
            //    .ToPropertyEx(this, x => x.IsConnected, false);

            DeviceService.DeviceInfoStream
                .Subscribe(d => DeviceInstance = d);

            //this.WhenAnyValue(x => x.ConnectionInfo)
            //    .Select(ci => ci.IsConnected)
            //    .ToPropertyEx(this, x => x.IsConnected);
        }
    }
}