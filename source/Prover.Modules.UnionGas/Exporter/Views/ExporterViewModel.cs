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
using DynamicData.Binding;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IExportVerificationTest _exporter;

        public ExporterViewModel(IScreenManager screenManager, 
            EvcVerificationTestService service,
            IVerificationTestService verificationTestService, 
            IDeviceRepository deviceRepository,
            VerificationTestReportGenerator reportService,
            IExportVerificationTest exporter,
            ILoginService<EmployeeDTO> loginService,
            Func<EvcVerificationTest, VerificationGridViewModel> verificationViewModelFactory)
        {
            _exporter = exporter;
            ScreenManager = screenManager;
            HostScreen = screenManager;

            DeviceTypes = deviceRepository.GetAll().ToList();
            DeviceTypes.Add(new AllDeviceType {Id = Guid.Empty, Name = "All"});

            FilterByTypeCommand = ReactiveCommand.Create<DeviceType, DeviceType>(f => f);

            var deviceFilter = FilterByTypeCommand.Select(BuildDeviceFilter);
            var includeExportedFilter = this.WhenAnyValue(x => x.IncludeExportedTests).Select(BuildIncludeExportedFilter);
            var includeArchivedFilter = this.WhenAnyValue(x => x.IncludeArchived).Select(BuildIncludeArchivedFilter);

            var sorter = deviceFilter.Merge(includeExportedFilter).Merge(includeArchivedFilter)
                .Select(_ => SortExpressionComparer<EvcVerificationTest>.Ascending(t => t.TestDateTime));
           
            service.FetchTests()
                .Connect()
           
                .Sort(SortExpressionComparer<EvcVerificationTest>.Descending(t => t.TestDateTime), resetThreshold: 0)
                .Filter(deviceFilter)
                .Filter(includeExportedFilter)
                .Filter(includeArchivedFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(verificationViewModelFactory.Invoke)
                .Bind(out var allNotExported)
                .DisposeMany()
                .Subscribe();
            VisibleTests = allNotExported;
        }
        public ReactiveCommand<VerificationGridViewModel, Unit> PrintReport { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, string> AddSignedOnUser { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, DateTime?> ArchiveVerification { get; protected set; }
        public ReactiveCommand<VerificationGridViewModel, DateTime?> ExportVerification { get; protected set; }

        [Reactive] public bool IncludeExportedTests { get; set; } = false;
        [Reactive] public bool IncludeArchived { get; set; } = false;

        public ReactiveCommand<DeviceType, DeviceType> FilterByTypeCommand { get; protected set; }

        public ReadOnlyObservableCollection<VerificationGridViewModel> VisibleTests { get; }

        public ICollection<DeviceType> DeviceTypes { get; }

        public IScreenManager ScreenManager { get; }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }

        private Func<EvcVerificationTest, bool> BuildDeviceFilter(DeviceType deviceType) => t =>
            t.Device.DeviceType.Id == deviceType.Id || deviceType.Id == Guid.Empty;

        private Func<EvcVerificationTest, bool> BuildIncludeExportedFilter(bool include) =>
            test => include || test.ExportedDateTime == null;

        private Func<EvcVerificationTest, bool> BuildIncludeArchivedFilter(bool include) =>
            test => include || test.ArchivedDateTime == null;
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

/*
 * void SetupRx()
            {
                PrintReport =
                    ReactiveCommand.CreateFromTask<VerificationGridViewModel, Unit>(async (vm) =>
                    {
                        var viewModel = await verificationTestService.GetVerificationTest(vm.Test);
                        await reportService.GenerateAndViewReport(viewModel);
                        return Unit.Default;
                    });

                AddSignedOnUser = ReactiveCommand.CreateFromTask<VerificationGridViewModel, string>(async (vm) =>
                {
                    if (loginService.User != null)
                    {
                        vm.Test.EmployeeId = loginService.User?.Id;
                        await service.AddOrUpdateVerificationTest(vm.Test);
                    }

                    return loginService.User?.Id;
                }, loginService.LoggedIn);
                
                var canExport = this.WhenAnyValue(x => x.HasJobId, x => x.EmployeeId,
                    (j, e) => j && !string.IsNullOrEmpty(e));
                ExportVerification = ReactiveCommand.CreateFromTask(async (vm) =>
                {
                    var success = await exporter.Export(vm.Test);

                    if (success)
                    {
                        Test.ExportedDateTime = DateTime.Now;
                        await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return Test.ExportedDateTime;
                }, canExport);

                HasJobId = !string.IsNullOrEmpty(Test.JobId);
                ExportVerification.ToPropertyEx(this, x => x.ExportedDateTime, Test.ExportedDateTime);
                //ExportVerification
                //    .Select(x => x != null)
                //    .ToPropertyEx(this, x => x.IsExported, Test.ExportedDateTime != null);

                ArchiveVerification = ReactiveCommand.CreateFromTask(async (vm) =>
                {
                    if (
                        await MessageInteractions.ShowYesNo.Handle(
                            "Are you sure you want to archive this test?"))
                    {
                        vm.Test.ArchivedDateTime = DateTime.Now;
                        var updated = await verificationTestService.AddOrUpdateVerificationTest(Test);
                    }

                    return Test.ArchivedDateTime;
                });
                ArchiveVerification
                    .ToPropertyEx(this, x => x.ArchivedDateTime, Test.ArchivedDateTime);
            }
 * */
