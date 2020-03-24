using Devices.Mobile.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace Devices.Mobile.ViewModels
{
    public class DevicesViewModel : BaseViewModel
    {
        [Reactive]
        public ObservableCollection<DeviceGetDto> DeviceNames { get; set; } = new ObservableCollection<DeviceGetDto>();

        [Reactive]
        public ReactiveCommand<Unit, Unit> GetDevicesCommand { get; set; }

        public DevicesViewModel() : base()
        {
            GetDevicesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                DeviceNames.Clear();

                var devices = await DeviceService.FetchDevices();

                devices.ToList()
                    .ForEach(x => DeviceNames.Add(x));
            });
        }
    }
}