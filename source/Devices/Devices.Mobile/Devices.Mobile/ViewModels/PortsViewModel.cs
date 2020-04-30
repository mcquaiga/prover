using Devices.Mobile.Services;
using Devices.Mobile.Views;
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
    public class PortsViewModel : BaseViewModel
    {
        public DeviceGetDto Device { get; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> GetPortsCommand { get; set; }

        //public ObservableCollection<string> PortNames { [ObservableAsProperty]get; }

        [Reactive]
        public ObservableCollection<string> PortNames { get; set; } = new ObservableCollection<string>();

        [Reactive]
        public string SelectedPort { get; set; }

        public PortsViewModel() : base()
        {
            GetPortsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                PortNames.Clear();

                var ports = await DeviceService.FetchCommPortNames();

                ports.ToList()
                    .ForEach(x => PortNames.Add(x));
            });
        }

        public PortsViewModel(DeviceGetDto device) : this()
        {
            Device = device;
        }
    }
}