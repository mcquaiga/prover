using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Client.Desktop.Wpf.Reports;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly EvcVerificationTestService _service;

        public ExporterViewModel(IScreenManager screenManager, EvcVerificationTestService service,
            VerificationTestService viewModelService, DeviceRepository deviceRepository,
            VerificationTestReportGenerator reportService)
        {
            _service = service;
            ScreenManager = screenManager;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.GetAll().ToList();
            DeviceTypes.Add(new AllDeviceType {Id = Guid.Empty, Name = "All"});

            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, DeviceType>(f => f);
            PrintReport =
                ReactiveCommand.CreateFromTask<EvcVerificationTest>(async test =>
                {
                    var viewModel = await viewModelService.GetVerificationTest(test);
                    await reportService.GenerateAndViewReport(viewModel);
                });

            var deviceFilter = FilterByTypeCommand.Select(BuildDeviceFilter);
            var includeFilter = this.WhenAnyValue(x => x.IncludeExportedTests).Select(BuildIncludeExportedFilter);

            _service.FetchTests()
                .Connect()
                .Filter(deviceFilter)
                .Filter(includeFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var allNotExported)
                .DisposeMany()
                .Subscribe();
            VisibleTests = allNotExported;
        }

        [Reactive] public bool IncludeExportedTests { get; set; } = false;

        public ReactiveCommand<EvcVerificationTest, Unit> PrintReport { get; set; }

        public ReactiveCommand<DeviceType, DeviceType> FilterByTypeCommand { get; protected set; }

        public ReadOnlyObservableCollection<EvcVerificationTest> VisibleTests { get; }

        public ICollection<DeviceType> DeviceTypes { get; }

        public IScreenManager ScreenManager { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildDeviceFilter(DeviceType deviceType) => t =>
            t.Device.DeviceType.Id == deviceType.Id || deviceType.Id == Guid.Empty;

        private Func<EvcVerificationTest, bool> BuildIncludeExportedFilter(bool include) =>
            test => include || test.ExportedDateTime == null;

        #region Nested type: AllDeviceType

        private class AllDeviceType : DeviceType
        {
            public override Type GetBaseItemGroupClass(Type itemGroupType) => throw new NotImplementedException();

            public override TGroup GetGroupValues<TGroup>(IEnumerable<ItemValue> itemValues) =>
                throw new NotImplementedException();

            public override ItemGroup GetGroupValues(IEnumerable<ItemValue> itemValues, Type groupType) =>
                throw new NotImplementedException();

            public override IEnumerable<ItemMetadata> GetItemMetadata<T>() => throw new NotImplementedException();
        }

        #endregion
    }
}