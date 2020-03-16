using Client.Desktop.Wpf.ViewModels;
using Prover.Application.Services;
using ReactiveUI;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Reports;
using Devices.Core.Interfaces;
using Devices.Core.Repository;
using Prover.Domain.EvcVerifications;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly EvcVerificationTestService _service;
        private readonly VerificationTestReportGenerator _reportService;

        public ExporterViewModel(IScreenManager screenManager, EvcVerificationTestService service, VerificationViewModelService viewModelService, DeviceRepository deviceRepository, VerificationTestReportGenerator reportService)
        {
            _service = service;
            _reportService = reportService;
            ScreenManager = screenManager;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.Devices.Where(d => d.IsHidden == false).ToObservable();
            //DeviceTypes.Add(new DeviceType() { Id = Guid.Empty, Name = "All" } );

            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, DeviceType>(f => f);
            PrintReport =
                ReactiveCommand.CreateFromTask<EvcVerificationTest>(async test =>
                {
                    var viewModel = await viewModelService.GetTest(test);
                    await _reportService.GenerateAndViewReport(viewModel);
                });

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

        public ReactiveCommand<EvcVerificationTest, Unit> PrintReport { get; set; }

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