using Client.Desktop.Wpf.ViewModels;
using Prover.Application.Services;
using ReactiveUI;
using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Reports;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly EvcVerificationTestService _service;
        private readonly VerificationTestReportGenerator _reportService;

        public ExporterViewModel(IScreenManager screenManager, EvcVerificationTestService service, VerificationTestService viewModelService, DeviceRepository deviceRepository, VerificationTestReportGenerator reportService)
        {
            _service = service;
            _reportService = reportService;
            ScreenManager = screenManager;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.GetAll().ToList();
            DeviceTypes.Add(new AllDeviceType() { Id = Guid.Empty, Name = "All" } );

            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, DeviceType>(f => f);
            PrintReport =
                ReactiveCommand.CreateFromTask<EvcVerificationTest>(async test =>
                {
                    var viewModel = await viewModelService.GetVerificationTest(test);
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

        public ICollection<DeviceType> DeviceTypes { get; }

        public IScreenManager ScreenManager { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildFilter(DeviceType deviceType)
        {
            return t => (t.Device.DeviceType.Id == deviceType.Id) || deviceType.Id == Guid.Empty;
        }

        private class AllDeviceType : DeviceType
        {
            
            public override Type GetBaseItemGroupClass(Type itemGroupType) => throw new NotImplementedException();

            public override TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) => throw new NotImplementedException();

            public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType) => throw new NotImplementedException();

            public override IEnumerable<ItemMetadata> GetItemMetadata<T>() => throw new NotImplementedException();
        }
    }
}