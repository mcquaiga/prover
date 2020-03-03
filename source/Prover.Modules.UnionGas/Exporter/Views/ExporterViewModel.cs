using Client.Desktop.Wpf.ViewModels;
using Prover.Application.Services;
using ReactiveUI;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Prover.Domain.EvcVerifications;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly EvcVerificationTestService _service;

        public ExporterViewModel(IScreenManager screenManager, EvcVerificationTestService service, DeviceRepository deviceRepository)
        {
            _service = service;
            ScreenManager = screenManager;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.Devices;
            //DeviceTypes.Add(new DeviceType() { Id = Guid.Empty, Name = "All" } );

            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, DeviceType>(f => f);

            var filter = FilterByTypeCommand.Select(BuildFilter);
            _service.FetchTests()
                .Connect(v => v.ExportedDateTime == null)
                .Filter(filter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var allNotExported)
                .DisposeMany()
                .Subscribe();
            VisibleTests = allNotExported;
        }

        public ReactiveCommand<DeviceType, DeviceType> FilterByTypeCommand { get; protected set; }

        public ReadOnlyObservableCollection<EvcVerificationTest> VisibleTests { get; }

        public ReadOnlyObservableCollection<DeviceType> DeviceTypes { get; }

        public IScreenManager ScreenManager { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildFilter(DeviceType deviceType)
        {
            return t => (t.Device.DeviceType.Id == deviceType.Id) || deviceType.Id == Guid.Empty;
        }
    }
}